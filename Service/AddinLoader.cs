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
using System.Linq;
using System.Text;
using System.Reflection;
using Dover.Framework.Factory;
using Castle.Core.Logging;
using Dover.Framework.Attribute;
using Dover.Framework.Log;
using Dover.Framework.DAO;
using Dover.Framework.Interface;

namespace Dover.Framework.Service
{
    public class AddinLoader : MarshalByRefObject, IAddinLoader
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

        void IAddinLoader.StartThis()
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
                throw e;
            }
        }

        void IAddinLoader.StartMenu(Assembly asm)
        {
            this.StartMenu(asm);
        }

        internal void StartMenu(Assembly asm)
        {
            string addin = asm.GetName().Name;
            Logger.Debug(String.Format(Messages.ConfigureMenu, addin));
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
                    ProcessAddInStartupAttribute(attrs, type, method);
                }
            }
        }

        void IAddinLoader.StartMenu()
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

        private void ProcessAddInStartupAttribute(object[] attrs, Type type, MethodInfo method = null)
        {
            List<MenuAttribute> menus = new List<MenuAttribute>();

            foreach (var attr in attrs)
            {
                Logger.Debug(DebugString.Format(Messages.ProcessingAttribute, attr, type));
                if (attr is MenuEventAttribute)
                {
                    ((MenuEventAttribute)attr).OriginalType = type;
                    ((MenuEventAttribute)attr).OriginalMethod = method;
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
