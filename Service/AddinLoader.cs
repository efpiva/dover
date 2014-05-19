using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using AddOne.Framework.Attribute;
using Castle.Core.Logging;
using AddOne.Framework.DAO;
using AddOne.Framework.Model.SAP;
using AddOne.Framework.Factory;
using AddOne.Framework.Model;
using System.IO;
using System.Xml.Linq;

namespace AddOne.Framework.Service
{
    class AddInRunner
    {
        private AssemblyInformation asm;

        internal AddInRunner(AssemblyInformation asm)
        {
            this.asm = asm;
        }

        internal void Run()
        {
            var setup = new AppDomainSetup();
            setup.ApplicationName = "AddOne.Inception";
            setup.ApplicationBase = Environment.CurrentDirectory;
            AppDomain domain = AppDomain.CreateDomain("AddOne.AddIn", null, setup);
            domain.ExecuteAssembly(asm.Name + ".exe");
        }
    }

    public class AddinLoader
    {

        public ILogger Logger { get; set; }

        private PermissionManager permissionManager;
        private BusinessOneDAO b1DAO;
        private BusinessOneUIDAO uiDAO;
        private MenuEventHandler menuHandler;

        public AddinLoader(PermissionManager permissionManager, 
            BusinessOneDAO b1DAO, BusinessOneUIDAO uiDAO,
            MenuEventHandler menuHandler)
        {
            this.permissionManager = permissionManager;
            this.b1DAO = b1DAO;
            this.uiDAO = uiDAO;
            this.menuHandler = menuHandler;
        }

        internal void LoadAddins(List<AssemblyInformation> addins)
        {
            var authorizedAddins = FilterAuthorizedAddins(addins);
            foreach (var addin in authorizedAddins)
            {
                var asm = Assembly.Load(addin.Name);
                RegisterObjects(asm);
                ConfigureAddin(addin);
                RegisterAddin(addin);
                ConfigureLog(addin);
            }
        }

        private void ConfigureLog(AssemblyInformation addin)
        {
            var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AddOneAddin.config");
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
                    Logger.Debug(String.Format(Messages.ProcessingAttribute, attr, type));
                    if (attr is ResourceBOMAttribute)
                    {
                        ProcessAddInAttribute((ResourceBOMAttribute)attr, assembly);
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

        private void ProcessAddInAttribute(ResourceBOMAttribute resourceBOMAttribute, Assembly asm)
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
            var thread = new Thread(new ThreadStart(runner.Run));
            thread.SetApartmentState(ApartmentState.STA);
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
            try
            {
                Assembly thisAsm = Assembly.GetEntryAssembly();
                RegisterObjects(thisAsm);
                var addin = thisAsm.FullName;
                Logger.Info(String.Format(Messages.ConfiguringAddin, addin));
                List<MenuAttribute> menus = new List<MenuAttribute>();
                var assembly = Assembly.GetEntryAssembly();
                var types = (from type in assembly.GetTypes()
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
            catch (Exception e)
            {
                Logger.Error(Messages.StartThisError, e);
            }
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
                Logger.Debug(String.Format(Messages.ProcessingAttribute, attr, type));
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
            }

            if (menus.Count > 0)
            {
                uiDAO.ProcessMenuAttribute(menus);
            }
        }


        internal void ShutdownAddins()
        {
            throw new NotImplementedException();
        }
    }
}
