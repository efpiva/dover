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
using System.IO;
using System.Reflection;
using System.Threading;
using Castle.Windsor;
using Dover.Framework.Attribute;
using Dover.Framework.Factory;
using Dover.Framework.Interface;
using Dover.Framework.Remoting;

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
    [AddIn(i18n="Dover.Framework.Messages.DoverName", Description="Dover Framework", Name="Framework")]
    [ResourceBOM("Dover.Framework.DatabaseTables.xml", ResourceType.UserTable)]
    [ResourceBOM("Dover.Framework.DatabaseFields.xml", ResourceType.UserField)]
    public class Application : MarshalByRefObject, IApplication
    {
        private IWindsorContainer appContainer;

        private string[] configFiles = { "Dover.config", "DoverTemp.config" };

        public Application()
        {
            CheckLogging();
        }

        private void CheckLogging()
        {
            foreach (var config in configFiles)
            {
                string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config);
                string logResource = "Dover.Framework." + config;
                if (!File.Exists(logFile))
                {
                    using (var doverConfig = Assembly.GetExecutingAssembly().GetManifestResourceStream(logResource))
                    {
                        using (var destinationStream = new FileStream(logFile, FileMode.CreateNew))
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
        }

        /// <summary>
        /// Release all resources.
        /// </summary>
        public void ShutDownApp()
        {
            if (appContainer != null)
            {
                IAppEventHandler appEventHandler = appContainer.Resolve<IAppEventHandler>();
                appEventHandler.ShutDown();
                appContainer = null;
            }
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
        void IApplication.RunAddin()
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
        void IApplication.RunInception()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }
            var boot = appContainer.Resolve<Boot>();
            boot.StartUp();
        }

        /// <summary>
        /// Used to probe if Framework was properly initialized. Used by unit tests mainly.
        /// </summary>
        public bool Initialized
        {
            get
            {
                if (appContainer == null)
                {
                    return false;
                }
                var microBoot = appContainer.Resolve<MicroBoot>();
                if (microBoot.InceptionAddinManager == null)
                    return false;

                return microBoot.InceptionAddinManager.Initialized;
            }
        }
    }
}
