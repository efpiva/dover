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
    internal class AddInRunner
    {
        internal AssemblyInformation asm;
        internal ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        internal AddinManager addinInceptionManager;
        internal B1SResourceManager addinB1SResourceManager;
        internal FormEventHandler addinFormEventHandler;

        internal Thread runnerThread;

        internal AddInRunner(AssemblyInformation asm)
        {
            this.asm = asm;
        }

        internal void Run()
        {
            var setup = new AppDomainSetup();
            setup.ApplicationName = "Dover.Inception";
            setup.ApplicationBase = Environment.CurrentDirectory;
            var domain = AppDomain.CreateDomain("Dover.AddIn", null, setup);
            domain.SetData("shutdownEvent", shutdownEvent); // Thread synchronization
            domain.SetData("assemblyName", asm.Name); // Used to get current AssemblyName for logging and reflection
            Application app = (Application)domain.CreateInstanceAndUnwrap("Framework", "Dover.Framework.Application");
            SAPServiceFactory.PrepareForInception(domain);
            addinInceptionManager = app.Resolve<AddinManager>();
            addinB1SResourceManager = app.Resolve<B1SResourceManager>();
            addinFormEventHandler = app.Resolve<FormEventHandler>();
            Sponsor<Application> appSponsor = new Sponsor<Application>(app);
            Sponsor<AddinManager> inceptionSponsor = new Sponsor<AddinManager>(addinInceptionManager);
            Sponsor<B1SResourceManager> b1sSponsor = new Sponsor<B1SResourceManager>(addinB1SResourceManager);
            Sponsor<FormEventHandler> formEventSponsor = new Sponsor<FormEventHandler>(addinFormEventHandler);
            app.RunAddin();
            AppDomain.Unload(domain);
        } 
    }

     [ServiceBehavior(
    ConcurrencyMode = ConcurrencyMode.Multiple,
    InstanceContextMode = InstanceContextMode.Single
  )]
    public class AddinManager : MarshalByRefObject
    {

        public ILogger Logger { get; set; }

        private PermissionManager permissionManager;
        private BusinessOneDAO b1DAO;
        private BusinessOneUIDAO uiDAO;
        private MenuEventHandler menuHandler;
        private List<AddInRunner> runningAddIns = new List<AddInRunner>();
        private ServiceHost host; // namedPipe server;
        private I18NService i18nService;

        public AddinManager(PermissionManager permissionManager, 
            BusinessOneDAO b1DAO, BusinessOneUIDAO uiDAO,
            MenuEventHandler menuHandler, I18NService i18nService)
        {
            this.permissionManager = permissionManager;
            this.b1DAO = b1DAO;
            this.uiDAO = uiDAO;
            this.menuHandler = menuHandler;
            this.i18nService = i18nService;
        }

        internal void LoadAddins(List<AssemblyInformation> addins)
        {
            var authorizedAddins = FilterAuthorizedAddins(addins);
            foreach (var addin in authorizedAddins)
            {
                var asm = Assembly.Load(addin.Name);
                if (!IsInstalled(addin.Code))
                {
                    RegisterObjects(asm);
                    MarkAsInstalled(addin.Code);
                }
                ConfigureAddin(addin);
                RegisterAddin(addin);
                ConfigureLog(addin);
            }
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
                        comments += tempComments + "\n";
                    }
                    else if (attr is PermissionAttribute)
                    {
                        CheckPermissionAttribute((PermissionAttribute)attr, out tempComments);
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
            try
            {
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
                        case ResourceType.FormattedSearch:
                            var fsBOM = b1DAO.GetBOMFromXML<FormattedSearchBOM>(resourceStream);
                            UpdateOutputValues(ref comments, fsBOM, Messages.FS);
                            break;
                        case ResourceType.QueryCategories:
                            var qcBOM = b1DAO.GetBOMFromXML<QueryCategoriesBOM>(resourceStream);
                            UpdateOutputValues(ref comments, qcBOM, Messages.QC);
                            break;
                        case ResourceType.UserQueries:
                            var uqBOM = b1DAO.GetBOMFromXML<UserQueriesBOM>(resourceStream);
                            UpdateOutputValues(ref comments, uqBOM, Messages.UQ);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format("Não foi possível processar atributo {0} do Addin.", resourceBOMAttribute), e);
            }
        }

        private void UpdateOutputValues(ref string comments, IBOM bom, string bomName)
        {
            List<object> keys = b1DAO.ListMissingBOMKeys(bom);

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
            Assembly assembly;

            try
            {
                assembly = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                where asm.GetName().Name == addin.Name
                                select asm).First();
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
                        ProcessAddInResourceAttribute((ResourceBOMAttribute)attr, assembly);
                    }
                    else if (attr is PermissionAttribute)
                    {
                        ProcessPermissionAttribute((PermissionAttribute)attr);
                    }
                }
            }
        }

        private void ProcessPermissionAttribute(PermissionAttribute permissionAttribute)
        {
            b1DAO.UpdateOrSavePermissionIfNotExists(permissionAttribute);
        }

        private void ProcessAddInResourceAttribute(ResourceBOMAttribute resourceBOMAttribute, Assembly asm)
        {
            try
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
                        case ResourceType.FormattedSearch:
                            var fsBOM = b1DAO.GetBOMFromXML<FormattedSearchBOM>(resourceStream);
                            b1DAO.UpdateOrSaveBOMIfNotExists(fsBOM);
                            break;
                        case ResourceType.QueryCategories:
                            var qcBOM = b1DAO.GetBOMFromXML<QueryCategoriesBOM>(resourceStream);
                            b1DAO.UpdateOrSaveBOMIfNotExists(qcBOM);
                            break;
                        case ResourceType.UserQueries:
                            var uqBOM = b1DAO.GetBOMFromXML<UserQueriesBOM>(resourceStream);
                            b1DAO.UpdateOrSaveBOMIfNotExists(uqBOM);
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error(String.Format("Não foi possível processar atributo {0} do Addin.", resourceBOMAttribute), e);
            }
        }

        private void RegisterAddin(AssemblyInformation addin)
        {
            AddInRunner runner = new AddInRunner(addin);
            runningAddIns.Add(runner);
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

        internal void StartThis()
        {
            string thisAsmName = (string)AppDomain.CurrentDomain.GetData("assemblyName");
            try
            {
                Assembly thisAsm = AppDomain.CurrentDomain.Load(thisAsmName);
                RegisterObjects(thisAsm);
                StartMenu(thisAsm);

            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Messages.StartThisError, thisAsmName), e);
            }
        }

        internal void ConfigureAddinsI18N()
        {
            i18nService.ConfigureThreadI18n(Thread.CurrentThread);
            foreach (var addin in runningAddIns)
            {
                i18nService.ConfigureThreadI18n(addin.runnerThread);
                addin.addinInceptionManager.StartMenu();
                addin.addinFormEventHandler.RegisterForms(false);
            }
        }

        private void StartMenu()
        {
            string thisAsmName = (string)AppDomain.CurrentDomain.GetData("assemblyName");
            try
            {
                Assembly thisAsm = AppDomain.CurrentDomain.Load(thisAsmName);
                StartMenu(thisAsm);
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(Messages.StartThisError, thisAsmName), e);
            }
        }

        private void StartMenu(Assembly asm)
        {
            string addin = asm.GetName().Name;
            Logger.Info(String.Format(Messages.ConfiguringAddin, addin));
            List<MenuAttribute> menus = new List<MenuAttribute>();
            var types = (from type in asm.GetTypes()
                         where type.IsClass
                         select type);

            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(true);
                ProcessAddInStartupAttribute(attrs, type);
                foreach (var method in type.GetMethods())
                {
                    attrs = method.GetCustomAttributes(true);
                    ProcessAddInStartupAttribute(attrs, type);
                }
            }
        }

         private bool IsInstalled(string code)
        {
            string installedFlag = b1DAO.ExecuteSqlForObject<string>(
                string.Format("SELECT ISNULL(U_Installed, 'N') from [@DOVER_MODULES] where Code = '{0}'", code));
            return !string.IsNullOrEmpty(installedFlag) && installedFlag == "Y";
        }

        private void RegisterObjects(Assembly thisAsm)
        {
            ContainerManager.RegisterAssembly(thisAsm);
            foreach (var asm in thisAsm.GetReferencedAssemblies())
            {
                if (permissionManager.AddInEnabled(asm.Name))
                {
                    ContainerManager.RegisterAssembly(Assembly.Load(asm));
                }
            }
        }

        private void ProcessAddInStartupAttribute(object[] attrs, Type type)
        {
            List<MenuAttribute> menus = new List<MenuAttribute>();

            foreach (var attr in attrs)
            {
                Logger.Debug(DebugString.Format(Messages.ProcessingAttribute, attr, type));
                if (attr is MenuEventAttribute)
                {
                    ((MenuEventAttribute)attr).OriginalType = type;
                    menuHandler.RegisterMenuEvent((MenuEventAttribute)attr);
                }
                else if (attr is MenuAttribute)
                {
                    ((MenuAttribute)attr).OriginalType = type;
                    menus.Add((MenuAttribute)attr);
                }
                else if (attr is AddInAttribute)
                {
                    string initMethod = ((AddInAttribute)attr).InitMethod;
                    if (!string.IsNullOrWhiteSpace(initMethod))
                    {
                        object obj = ContainerManager.Container.Resolve(type);
                        type.InvokeMember(initMethod, BindingFlags.InvokeMethod, null, obj, null);
                    }
                }
            }

            if (menus.Count > 0)
            {
                uiDAO.ProcessMenuAttribute(menus);
            }
        }


        public void ShutdownAddins()
        {
            foreach (var runner in runningAddIns)
            {
                Logger.Info(string.Format(Messages.ShutdownAddin, runner.asm.Name));
                runner.shutdownEvent.Set();
            }
            runningAddIns = new List<AddInRunner>(); // clean running AddIns.
            System.Windows.Forms.Application.Exit(); // free main Inception thread.
        }
    }
}
