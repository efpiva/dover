/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Xml.Linq;
using Dover.Framework.Attribute;
using Dover.Framework.DAO;
using Dover.Framework.Factory;
using Dover.Framework.Model;
using Dover.Framework.Model.SAP;
using Dover.Framework.Remoting;
using Castle.Core.Logging;
using Dover.Framework.Log;

namespace Dover.Framework.Service
{
    internal enum AddinStatus
    {
        Running,
        Stopped
    }

    public class ConfigAddin : MarshalByRefObject
    {

        public ILogger Logger { get; set; }
        public BusinessOneDAO b1DAO { get; set; }

        internal void ConfigureAddin(AssemblyInformation addin)
        {
            List<ResourceBOMAttribute> resourceAttr = new List<ResourceBOMAttribute>();
            Assembly assembly;

            try
            {
                assembly = Assembly.Load(addin.Name);
            }
            catch (InvalidOperationException e)
            {
                Logger.Error(String.Format(Messages.AddInNotFound, addin), e);
                return;
            }

            var types = (from type in assembly.GetTypes()
                         where type.IsClass
                         select type);

            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    Logger.Debug(DebugString.Format(Messages.ProcessingAttribute, attr, type));
                    if (attr is ResourceBOMAttribute)
                    {
                        resourceAttr.Add((ResourceBOMAttribute)attr);
                    }
                    else if (attr is PermissionAttribute)
                    {
                        b1DAO.UpdateOrSavePermissionIfNotExists((PermissionAttribute)attr);
                    }
                }
            }
            resourceAttr.Sort(); // need to order becase we need create tables, than fields and at the end UDOs.
            foreach (var resource in resourceAttr)
            {
                ProcessAddInResourceAttribute(resource, assembly);
            }
        }
        
        private void ProcessAddInResourceAttribute(ResourceBOMAttribute resourceBOMAttribute, Assembly asm)
        {
            using (var resourceStream = asm.GetManifestResourceStream(resourceBOMAttribute.ResourceName))
            {
                if (resourceStream == null)
                {
                    Logger.Error(string.Format(Messages.InternalResourceMissing, resourceBOMAttribute.ResourceName));
                    return;
                }
                switch (resourceBOMAttribute.Type)
                {
                    case ResourceType.UserField:
                        var userFieldBOM = b1DAO.GetBOMFromXML<UserFieldBOM>(resourceStream);
                        b1DAO.SaveBOMIfNotExists(userFieldBOM);
                        break;
                    case ResourceType.UserTable:
                        var userTableBOM = b1DAO.GetBOMFromXML<UserTableBOM>(resourceStream);
                        b1DAO.SaveBOMIfNotExists(userTableBOM);
                        break;
                    case ResourceType.UDO:
                        var udoBOM = b1DAO.GetBOMFromXML<UDOBOM>(resourceStream);
                        b1DAO.UpdateOrSaveBOMIfNotExists(udoBOM);
                        break;
                }
            }
        }

    }

    internal class AddInRunner
    {
        internal AssemblyInformation asm;
        internal ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        /// <summary>
        /// This is the AddinManager of the Framework (creator of all Addins AppDomains)
        /// </summary>
        internal AddinManager frameworkAddinManager;
        internal B1SResourceManager addinB1SResourceManager;
        internal FormEventHandler addinFormEventHandler;
        internal EventDispatcher eventDispatcher;
        internal AddinLoader addinLoader;

        internal Thread runnerThread;

        internal AddInRunner(AssemblyInformation asm, AddinManager frameworkAddinManager)
        {
            this.asm = asm;
            this.frameworkAddinManager = frameworkAddinManager;
        }

        internal void Run()
        {
            var setup = new AppDomainSetup();
            setup.ApplicationName = "Dover.Inception";
            setup.ApplicationBase = Environment.CurrentDirectory;
            var domain = AppDomain.CreateDomain("Dover.AddIn", null, setup);
            domain.SetData("shutdownEvent", shutdownEvent); // Thread synchronization
            domain.SetData("assemblyName", asm.Name); // Used to get current AssemblyName for logging and reflection
            domain.SetData("frameworkManager", frameworkAddinManager); 
            Application app = (Application)domain.CreateInstanceAndUnwrap("Framework", "Dover.Framework.Application");
            SAPServiceFactory.PrepareForInception(domain);
            addinB1SResourceManager = app.Resolve<B1SResourceManager>();
            addinFormEventHandler = app.Resolve<FormEventHandler>();
            addinLoader = app.Resolve<AddinLoader>();
            eventDispatcher = app.Resolve<EventDispatcher>();
            Sponsor<Application> appSponsor = new Sponsor<Application>(app);
            Sponsor<B1SResourceManager> b1sSponsor = new Sponsor<B1SResourceManager>(addinB1SResourceManager);
            Sponsor<FormEventHandler> formEventSponsor = new Sponsor<FormEventHandler>(addinFormEventHandler);
            Sponsor<AddinLoader> loaderSponsor = new Sponsor<AddinLoader>(addinLoader);
            Sponsor<EventDispatcher> eventSponsor = new Sponsor<EventDispatcher>(eventDispatcher);
            app.RunAddin();
            AppDomain.Unload(domain);
        } 
    }

    public class AddinManager : MarshalByRefObject
    {
        public ILogger Logger { get; set; }
        private PermissionManager permissionManager;
        private AssemblyDAO assemblyDAO;
        private AssemblyManager assemblyManager;
        private BusinessOneDAO b1DAO;
        private List<AddInRunner> runningAddIns = new List<AddInRunner>();
        private Dictionary<string, AddInRunner> runningAddinsHash = new Dictionary<string, AddInRunner>();
         
        private I18NService i18nService;

        public AddinManager(PermissionManager permissionManager, AssemblyManager assemblyManager,
            BusinessOneDAO b1DAO, I18NService i18nService, AssemblyDAO assemblyDAO)
        {
            this.permissionManager = permissionManager;
            this.assemblyDAO = assemblyDAO;
            this.b1DAO = b1DAO;
            this.i18nService = i18nService;
            this.assemblyManager = assemblyManager;
        }

        internal void LoadAddins(List<AssemblyInformation> addins)
        {
            var authorizedAddins = FilterAuthorizedAddins(addins);
            foreach (var addin in authorizedAddins)
            {
                LoadAddin(addin);
            }
        }

        private void LoadAddin(AssemblyInformation addin)
        {
            if (!IsInstalled(addin.Code))
            {
                InstallAddin(addin);
            }
            RegisterAddin(addin);
            ConfigureLog(addin);
            
        }

        private void InstallAddin(AssemblyInformation addin)
        {
            try
            {
                ConfigureAddin(addin);
                MarkAsInstalled(addin.Code);
            }
            catch (Exception e)
            {
                MarkAsNotInstalled(addin.Code);
                throw e;
            }
        }

        private void MarkAsNotInstalled(string addInCode)
        {
            b1DAO.ExecuteStatement(
    string.Format("UPDATE [@DOVER_MODULES] set U_Installed = 'N' where Code = '{0}'", addInCode));
        }

        private void MarkAsInstalled(string addInCode)
        {
            b1DAO.ExecuteStatement(
    string.Format("UPDATE [@DOVER_MODULES] set U_Installed = 'Y' where Code = '{0}'", addInCode));
        }

        private void ConfigureLog(AssemblyInformation addin)
        {
            var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DoverAddin.config");
            var destination = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, addin.Name + ".config");

            if (File.Exists(source) && !File.Exists(destination)) 
            {
                var doc = XDocument.Load(source);

                var query = from c in doc.Elements("configuration").Elements("log4net")
                            .Elements("appender").Elements("file")
                            select c;

                foreach (var fileTag in query)
                {
                    fileTag.Attribute("value").Value = addin.Name + ".log";
                }

                doc.Save(destination);
            }
        }

        internal bool CheckAddinConfiguration(string assemblyName, out string comments)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            bool isValid = false;
            string tempComments = string.Empty;
            comments = string.Empty;

            var types = (from type in assembly.GetTypes()
                         where type.IsClass
                         select type);

            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    Logger.Debug(DebugString.Format(Messages.ProcessingAttribute, attr, type));
                    if (attr is ResourceBOMAttribute)
                    {
                        CheckAddInAttribute((ResourceBOMAttribute)attr, assembly, out tempComments);
                        if (!string.IsNullOrEmpty(tempComments))
                            comments += tempComments + "\n";
                    }
                    else if (attr is PermissionAttribute)
                    {
                        CheckPermissionAttribute((PermissionAttribute)attr, out tempComments);
                        if (!string.IsNullOrEmpty(tempComments))
                            comments += tempComments + "\n";
                    }
                    else if (attr is AddInAttribute && !string.IsNullOrWhiteSpace(((AddInAttribute)attr).Description))
                    {
                        isValid = true;
                    }
                }
            }

            return isValid;
        }

        private void CheckPermissionAttribute(PermissionAttribute permissionAttribute, out string comments)
        {
            comments = string.Empty;

            if (!b1DAO.PermissionExists(permissionAttribute))
            {
                comments += string.Format("{0}: ", Messages.Permission);
                comments += permissionAttribute.PermissionID;
            }
        }

        private void CheckAddInAttribute(ResourceBOMAttribute resourceBOMAttribute, Assembly asm, out string comments)
        {
            comments = string.Empty;
            using (var resourceStream = asm.GetManifestResourceStream(resourceBOMAttribute.ResourceName))
            {
                if (resourceStream == null)
                {
                    Logger.Error(string.Format(Messages.InternalResourceMissing, resourceBOMAttribute.ResourceName));
                }
                switch (resourceBOMAttribute.Type)
                {
                    case ResourceType.UserField:
                        var userFieldBOM = b1DAO.GetBOMFromXML<UserFieldBOM>(resourceStream);
                        UpdateOutputValues(ref comments, userFieldBOM, Messages.UserField);
                        break;
                    case ResourceType.UserTable:
                        var userTableBOM = b1DAO.GetBOMFromXML<UserTableBOM>(resourceStream);
                        UpdateOutputValues(ref comments, userTableBOM, Messages.UserTable);
                        break;
                    case ResourceType.UDO:
                        var udoBOM = b1DAO.GetBOMFromXML<UDOBOM>(resourceStream);
                        UpdateOutputValues(ref comments, udoBOM, Messages.UDO);
                        break;
                }
            }
        }

        private void UpdateOutputValues(ref string comments, IBOM bom, string bomName)
        {
            List<string> keys = b1DAO.ListMissingBOMKeys(bom);

            if (keys.Count > 0)
            {
                comments += string.Format("{0}: ", bomName);
                bool first = true;
                foreach (var key in keys)
                {
                    if (!first)
                        comments += ", ";
                    else
                        first = false;

                    comments += key;
                }
                comments += "\n";
            }
            else
            {
                comments = string.Empty;
            }
        }

        private void ConfigureAddin(AssemblyInformation addin)
        {
            Logger.Info(String.Format(Messages.ConfiguringAddin, addin));
            var setup = new AppDomainSetup();
            setup.ApplicationName = "Dover.ConfigureDomain";
            setup.ApplicationBase = Environment.CurrentDirectory;
            AppDomain configureDomain = AppDomain.CreateDomain("ConfigureDomain", null, setup);
            try
            {
                Application app = (Application)configureDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                    "Dover.Framework.Application");
                SAPServiceFactory.PrepareForInception(configureDomain);
                ConfigAddin addinConfig = app.Resolve<ConfigAddin>();
                addinConfig.ConfigureAddin(addin);
            }
            finally
            {
                AppDomain.Unload(configureDomain);
            }
        }

        private void RegisterAddin(AssemblyInformation addin)
        {
            AddInRunner runner = new AddInRunner(addin, this);
            runningAddIns.Add(runner);
            runningAddinsHash.Add(addin.Name, runner);
            var thread = new Thread(new ThreadStart(runner.Run));
            thread.SetApartmentState(ApartmentState.STA);
            i18nService.ConfigureThreadI18n(thread);
            runner.runnerThread = thread;
            thread.Start();
        }

        private List<AssemblyInformation> FilterAuthorizedAddins(List<AssemblyInformation> addins)
        {
            var authorized = new List<AssemblyInformation>();
            foreach (var addin in addins)
            {
                if (permissionManager.AddInEnabled(addin.Name))
                    authorized.Add(addin);
            }
            return authorized;
        }

        internal void ConfigureAddinsI18N()
        {
            i18nService.ConfigureThreadI18n(Thread.CurrentThread);
            foreach (var addin in runningAddIns)
            {
                i18nService.ConfigureThreadI18n(addin.runnerThread);
                addin.addinLoader.StartMenu();
                addin.addinFormEventHandler.RegisterForms(false);
            }
        }

         private bool IsInstalled(string code)
        {
            string installedFlag = b1DAO.ExecuteSqlForObject<string>(
                string.Format("SELECT ISNULL(U_Installed, 'N') from [@DOVER_MODULES] where Code = '{0}'", code));
            return !string.IsNullOrEmpty(installedFlag) && installedFlag == "Y";
        }

        internal void ShutdownAddins()
        {
            foreach (var runner in runningAddIns)
            {
                Logger.Info(string.Format(Messages.ShutdownAddin, runner.asm.Name));
                runner.eventDispatcher.UnregisterEvents();
                runner.addinFormEventHandler.UnRegisterForms();
                runner.shutdownEvent.Set();
            }
            runningAddIns = new List<AddInRunner>(); // clean running AddIns.
            runningAddinsHash = new Dictionary<string, AddInRunner>(); // clean-up.
            System.Windows.Forms.Application.Exit(); // free main Inception thread.
        }

        internal AddinStatus GetAddinStatus(string name)
        {
            return (runningAddinsHash.ContainsKey(name)) ? AddinStatus.Running : AddinStatus.Stopped;
        }

        internal void ShutdownAddin(string name)
        {
            var runningBackup = runningAddIns;
            var runningHashBackup = runningAddinsHash;
            try
            {
                AddInRunner addin;
                runningAddinsHash.TryGetValue(name, out addin);
                if (addin != null)
                {
                    Logger.Info(string.Format(Messages.ShutdownAddin, addin.asm.Name));
                    addin.eventDispatcher.UnregisterEvents();
                    addin.addinFormEventHandler.UnRegisterForms();
                    addin.shutdownEvent.Set();
                    runningAddIns.Remove(addin);
                    runningAddinsHash.Remove(name);
                }
            }
            catch (Exception e)
            {
                // rollback memory state.
                runningAddIns = runningBackup;
                runningAddinsHash = runningHashBackup;
                throw e;
            }
        }

        internal void StartAddin(string name)
        {
            AssemblyInformation asmInfo = assemblyDAO.GetAssemblyInformation(name, "A");
            assemblyManager.UpdateAppDataFolder(asmInfo, AppDomain.CurrentDomain.BaseDirectory);
            LoadAddin(asmInfo);
        }

        internal void InstallAddin(string name)
        {
            AssemblyInformation asmInfo = assemblyDAO.GetAssemblyInformation(name, "A");
            assemblyManager.UpdateAppDataFolder(asmInfo, AppDomain.CurrentDomain.BaseDirectory);
            InstallAddin(asmInfo);
        }
    }
}
