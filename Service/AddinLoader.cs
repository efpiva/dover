using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Dover.Framework.Factory;
using Castle.Core.Logging;
using Dover.Framework.Attribute;
using Dover.Framework.Log;
using Dover.Framework.DAO;

namespace Dover.Framework.Service
{
    public class AddinLoader : MarshalByRefObject
    {
        public ILogger Logger { get; set; }
        private PermissionManager permissionManager;
        private MenuEventHandler menuHandler;
        private BusinessOneUIDAO uiDAO;

        public AddinLoader(MenuEventHandler menuHandler, PermissionManager permissionManager,
             BusinessOneUIDAO uiDAO)
        {
            this.permissionManager = permissionManager;
            this.uiDAO = uiDAO;
            this.menuHandler = menuHandler;
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

        internal void StartMenu(Assembly asm)
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

        internal void StartMenu()
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
    }
}
