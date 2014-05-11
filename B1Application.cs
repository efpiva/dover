using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.Logging;
using AddOne.Framework.Factory;
using AddOne.Framework.Log;
using Castle.Core.Logging;
using AddOne.Framework.DAO;
using AddOne.Framework.Service;
using AddOne.Framework.Factory;
using System.Reflection;
using SAPbouiCOM.Framework;
using AddOne.Framework.Monad;

namespace AddOne.Framework
{
    public class B1Application
    {
        private string appDomainFolder;
        private IWindsorContainer appContainer;

        public B1Application()
        {
        }

        public void ShutDownApp()
        {
            appContainer.Dispose();
        }

        public T[] ResolveAll<T>()
        {
            if (appContainer == null)
                return null;

            return appContainer.ResolveAll<T>();
        }
        
        public T Resolve<T>()
        {
            if (appContainer == null)
                return default(T);

            return appContainer.Resolve<T>();
        }

        public void StartApp()
        {
            appContainer = ContainerManager.BuildContainer();
        }

        public void Run()
        {
            if (AppDomain.CurrentDomain.FriendlyName != "AddOne.AddIn"
                && AppDomain.CurrentDomain.FriendlyName != "AddOne.Inception")
            {
                var container = ContainerManager.BuildContainer();
                var microCore = container.Resolve<MicroCore>();
                appDomainFolder = microCore.PrepareFramework();
                container.Dispose();
            }

            if (appDomainFolder != null)
            {
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationName = "AddOne.Inception";
                setup.ApplicationBase = appDomainFolder;
                AppDomain inception = AppDomain.CreateDomain("AddOne.Inception", null, setup);
                Environment.CurrentDirectory = appDomainFolder;
                SAPServiceFactory.PrepareForInception(inception);
                inception.ExecuteAssembly("AddOne.exe", Environment.GetCommandLineArgs());
            }
            else if (AppDomain.CurrentDomain.FriendlyName == "AddOne.AddIn")
            {
                var container = ContainerManager.BuildContainer();
                var loader = container.Resolve<Boot>();
                loader.StartThis();
                container.Dispose();
            } 
            else
            {
                var container = ContainerManager.BuildContainer();
                var boot = container.Resolve<Boot>();
                boot.StartUp();
                container.Dispose();
            }
        }
    }
}
