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

namespace AddOne.Framework
{
    public class B1Application
    {
        private string appDomainFolder;

        public B1Application()
        {
            if (AppDomain.CurrentDomain.FriendlyName == "AddOne.AddIn"
                || AppDomain.CurrentDomain.FriendlyName == "AddOne.Inception")
                return;

            var container = ContainerManager.BuildContainer();
            var microCore = container.Resolve<MicroCore>();
            appDomainFolder = microCore.PrepareFramework();
            container.Dispose();
        }

        public void Run()
        {
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
                var loader = container.Resolve<AddinLoader>();
                var app = container.Resolve<Application>();
                loader.StartThis();
                app.Run();
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
