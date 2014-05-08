using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using AddOne.Framework.DAO;
using Castle.Facilities.Logging;
using Castle.Core.Logging;
using AddOne.Framework.Log;
using System.Reflection;

namespace AddOne.Framework.Factory
{
    internal class ContainerManager
    {
        public static IWindsorContainer Container { get; private set;}

        internal static WindsorContainer BuildContainer()
        {
            WindsorContainer container = new WindsorContainer();
            container.Register(Component.For<SAPbouiCOM.Application>().UsingFactoryMethod(SAPServiceFactory.ApplicationFactory));
            container.Register(Component.For<SAPbouiCOM.Framework.Application>().UsingFactoryMethod(SAPServiceFactory.FrameworkApplicationFactory));
            container.Register(Component.For<SAPbobsCOM.Company>().UsingFactoryMethod(SAPServiceFactory.CompanyFactory));

            container.Register(Component.For<BusinessOneDAO>().ImplementedBy<BusinessOneDAOSQLImpl>());
            container.Register(Component.For<BusinessOneUIDAO>().ImplementedBy<BusinessOneUIDAOImpl>());
            container.Register(Component.For<LicenseDAO>().ImplementedBy<LicenseDAOSQLImpl>());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(Environment.CurrentDirectory))
                        .IncludeNonPublicTypes().Pick()
                        .WithService.DefaultInterfaces().LifestyleTransient());
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
            // Factory injections.

            var logger = container.Resolve<ILogger>();
            SAPServiceFactory.Logger = logger;

            var b1dao = container.Resolve<BusinessOneDAO>();
            SAPAppender.B1DAO = b1dao;

            Container = container;
            return container;
        }

   }
}
