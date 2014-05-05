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
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using System.IO;

namespace AddOne.Framework.Factory
{

    public class ContainerManager
    {
        public static Func<SAPbobsCOM.Company>[] customCompanyFactory {get; set; }
        public static IWindsorContainer Container { get; private set; }

        internal static WindsorContainer BuildContainer()
        {
            Func<SAPbobsCOM.Company>[] companyFactory =  (customCompanyFactory == null) 
                ? new Func<SAPbobsCOM.Company>[] {SAPServiceFactory.CompanyFactory} : customCompanyFactory;

            WindsorContainer container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));

            container.Register(Component.For<SAPbouiCOM.Application>().UsingFactoryMethod(SAPServiceFactory.ApplicationFactory));
            container.Register(Component.For<SAPbouiCOM.Framework.Application>().UsingFactoryMethod(SAPServiceFactory.FrameworkApplicationFactory));

            for(int i=0 ; i< companyFactory.Length ; i++)
                container.Register(Component.For<SAPbobsCOM.Company>().UsingFactoryMethod(companyFactory[i]).Named("company" + i));

            for (int i = 0; i < companyFactory.Length; i++)
                container.Register(Component.For<BusinessOneDAO>().ImplementedBy<BusinessOneDAOSQLImpl>()
                    .DependsOn(Dependency.OnComponent(typeof(SAPbobsCOM.Company), "company" + i)).Named("b1dao" + i));
            container.Register(Component.For<BusinessOneUIDAO>().ImplementedBy<BusinessOneUIDAOImpl>());

            string runningFolder = Path.GetDirectoryName( Assembly.GetEntryAssembly().Location );


            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(runningFolder))
                        .IncludeNonPublicTypes().Pick()
                        .WithService.DefaultInterfaces().LifestyleTransient());
            container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
            // Factory injections.

            var logger = container.Resolve<ILogger>();
            logger.Debug(String.Format(Messages.StartupFolder, runningFolder));
            SAPServiceFactory.Logger = logger;

            var b1dao = container.Resolve<BusinessOneDAO>();
            SAPAppender.B1DAO = b1dao;

            Container = container;

            return container;
        }

   }
}
