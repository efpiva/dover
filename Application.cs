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
using Castle.Windsor;
using Dover.Framework.Factory;
using System.Threading;
using Dover.Framework.Remoting;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Dover.Framework.Attribute;

/*! \mainpage Dover Framework public API
 *
 * \section intro_sec Introduction
 *
 * This page describe the Dover Framework public API. For more information about dover visit <a href="http://efpiva.github.io">Dover Github Page</a>
 *
 */
namespace Dover.Framework
{
    /// <summary>
    /// Main entry point for the Dover Framework. During Application creation all resources
    /// are configured and assembly dependencies are registered in the AppDomain.
    /// 
    /// If you do not want to run it inside SAP Business One UI API, you can create an Application
    /// object and resolve your business logic service class using the Resolve<> method.
    /// 
    /// Be aware that doing this, you need to register your custom SAP Business One DI API resolver in
    /// Dover.Framework.Container.ContainerManager.
    /// 
    /// </summary>
    [AddIn(i18n="Dover.Framework.Messages.DoverName", Description="Dover Framework")]
    [ResourceBOM("Dover.Framework.DatabaseTables.xml", ResourceType.UserTable)]
    [ResourceBOM("Dover.Framework.DatabaseFields.xml", ResourceType.UserField)]
    public class Application : MarshalByRefObject
    {
        private IWindsorContainer appContainer;
        private static Dictionary<string, Assembly> assemblyCacheResolver = new Dictionary<string, Assembly>();
        private string[] embeddedAssemblies = {
            "Dover.Framework.Assemblies.SAPbouiCOM.dll",
            "Dover.Framework.Assemblies.log4net.dll",
            "Dover.Framework.Assemblies.Castle.Core.dll",
            "Dover.Framework.Assemblies.Castle.Facilities.Logging.dll",
            "Dover.Framework.Assemblies.Castle.Services.Logging.Log4netIntegration.dll",
            "Dover.Framework.Assemblies.Castle.Windsor.dll",
            "Dover.Framework.Assemblies.ICSharpCode.SharpZipLib.dll"};

        private string doverLogTemplate = "Dover.Framework.Dover.config";

        public Application()
        {
            // load all embedded resource into memory;
            byte[] ba = null;
            Assembly curAsm = Assembly.GetExecutingAssembly();
            foreach (string resource in embeddedAssemblies)
            {
                using (Stream stm = curAsm.GetManifestResourceStream(resource))
                {
                    ba = new byte[(int)stm.Length];
                    stm.Read(ba, 0, (int)stm.Length);
                    Assembly asm = Assembly.Load(ba);
                    assemblyCacheResolver.Add(asm.FullName, asm);
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            string bobsInteropPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Interop.SAPbobsCOM.dll");
            if (!File.Exists(bobsInteropPath))
            {
                using (Stream stm = curAsm.GetManifestResourceStream("Dover.Framework.Assemblies.Interop.SAPbobsCOM.dll"))
                {
                    ba = new byte[(int)stm.Length];
                    stm.Read(ba, 0, (int)stm.Length);
                    File.WriteAllBytes(bobsInteropPath, ba);
                }
            }

            CheckLogging();
        }

        private void CheckLogging()
        {
            string logging = Path.Combine(Environment.CurrentDirectory, "Dover.config");
            if (!File.Exists(logging))
            {
                using (var doverConfig = Assembly.GetExecutingAssembly().GetManifestResourceStream(doverLogTemplate))
                {
                    using (var destinationStream = new FileStream(logging, FileMode.CreateNew))
                    {
                        byte[] buffer = new byte[1000];
                        int buffLen;
                        while ((buffLen = doverConfig.Read(buffer, 0, 1000)) > 0)
                        {
                            destinationStream.Write(buffer, 0, buffLen);
                        }

                    }
                }
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (assemblyCacheResolver.ContainsKey(args.Name))
                return assemblyCacheResolver[args.Name];
            return null;
        }

        /// <summary>
        /// Release all resources.
        /// </summary>
        public void ShutDownApp()
        {
            appContainer.Dispose();
        }

        /// <summary>
        /// Return an array of all implementations for an specific service.
        /// </summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        /// <returns>Array of all implementations for the specified service.</returns>
        public T[] ResolveAll<T>()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }

            return appContainer.ResolveAll<T>();
        }

        /// <summary>
        /// Return the default implementation for an specific service.
        /// </summary>
        /// <typeparam name="T">Type of service.</typeparam>
        /// <returns>Object implementation registered on IoC container.</returns>
        public T Resolve<T>()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }

            return appContainer.Resolve<T>();
        }

        /// <summary>
        /// Start point for Dover Addon.
        /// </summary>
        public void Run()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }
            var microCore = appContainer.Resolve<MicroCore>();
            microCore.PrepareFramework();
        }

        /// <summary>
        /// Called for each addin that is started by Dover.
        /// </summary>
        internal void RunAddin()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }
            var loader = appContainer.Resolve<Boot>();
            if (loader.StartThis()) // in case of error, stop addin but do not close Dover.
            {
                ManualResetEvent shutdownEvent = (ManualResetEvent)AppDomain.CurrentDomain.GetData("shutdownEvent");
                Sponsor<ManualResetEvent> shutdownEventSponsor = new Sponsor<ManualResetEvent>(shutdownEvent);
                shutdownEvent.WaitOne(); // Wait until shutdown event is signaled.
            }
        }

        /// <summary>
        /// Called before all resources are loaded on AppDomain. This is done so we can guarantee
        /// that all code, including Framework code, is running using Database registered assembly Version.
        /// </summary>
        internal void RunInception()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }
            var boot = appContainer.Resolve<Boot>();
            boot.StartUp();
            appContainer.Dispose();
        }
    }
}
