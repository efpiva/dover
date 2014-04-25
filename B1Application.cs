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
using System.Reflection;

namespace AddOne.Framework
{
    public class B1Application
    {
        private string appDomainFolder;

        private static WindsorContainer BuildContainer()
        {
            WindsorContainer container = new WindsorContainer();
            container.Register(Component.For<SAPbouiCOM.Application>().UsingFactoryMethod(SAPServiceFactory.ApplicationFactory));
            container.Register(Component.For<SAPbouiCOM.Framework.Application>().UsingFactoryMethod(SAPServiceFactory.FrameworkApplicationFactory));
            container.Register(Component.For<SAPbobsCOM.Company>().UsingFactoryMethod(SAPServiceFactory.CompanyFactory));
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(Environment.CurrentDirectory))
                        .IncludeNonPublicTypes().InSameNamespaceAs<MicroCore>(true)
                        .WithService.DefaultInterfaces().LifestyleSingleton());
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
            // Factory injections.

            var logger = container.Resolve<ILogger>();
            SAPServiceFactory.Logger = logger;

            var b1dao = container.Resolve<BusinessOneDAO>();
            SAPAppender.B1DAO = b1dao;

            return container;
        }

        public B1Application()
        {
            if (AppDomain.CurrentDomain.FriendlyName == "AddOne.AddIn"
                || AppDomain.CurrentDomain.FriendlyName == "AddOne.Inception")
                return;

            var container = BuildContainer();
            var microCore = container.Resolve<MicroCore>();
            appDomainFolder = microCore.PrepareFramework();
            container.Dispose();
        }

        public void Run()
        {
            if (appDomainFolder != null)
            {
                AppDomain inception = AppDomain.CreateDomain("AddOne.Inception");
                inception.SetData("APPBASE", appDomainFolder);
                Environment.CurrentDirectory = appDomainFolder;
                SAPServiceFactory.PrepareForInception(inception);
                inception.ExecuteAssembly("AddOne.exe", Environment.GetCommandLineArgs());
            }
            else if (AppDomain.CurrentDomain.FriendlyName == "AddOne.AddIn")
            {
                var container = BuildContainer();
                var loader = container.Resolve<AddinLoader>();
                loader.StartThis();
                container.Dispose();
            } 
            else
            {
                var container = BuildContainer();
                var boot = container.Resolve<Boot>();
                boot.StartUp();
                container.Dispose();
            }
        }
    }
}
