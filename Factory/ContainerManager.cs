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
using AddOne.Framework.Service;

namespace AddOne.Framework.Factory
{

    public class ContainerManager
    {
        public static Func<SAPbobsCOM.Company>[] customCompanyFactory {get; set; }
        public static IWindsorContainer Container { get; private set; }

        internal static void RegisterAssembly(Assembly addIn)
        {
                Container.Register(Classes.FromAssembly(addIn)
                            .IncludeNonPublicTypes().Pick()
                            .WithService.DefaultInterfaces().LifestyleTransient());
        }

        internal static IWindsorContainer BuildContainer()
        {
            Func<SAPbobsCOM.Company>[] companyFactory =  (customCompanyFactory == null) 
                ? new Func<SAPbobsCOM.Company>[] {SAPServiceFactory.CompanyFactory} : customCompanyFactory;

            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(new ArrayResolver(Container.Kernel));
            Container.Register(Component.For<LicenseDAO>().ImplementedBy<LicenseDAOSQLImpl>());

            Container.Register(Component.For<SAPbouiCOM.Application>().UsingFactoryMethod(SAPServiceFactory.ApplicationFactory));
            Container.Register(Component.For<SAPbouiCOM.Framework.Application>().UsingFactoryMethod(SAPServiceFactory.FrameworkApplicationFactory));

            for(int i=0 ; i< companyFactory.Length ; i++)
                Container.Register(Component.For<SAPbobsCOM.Company>().UsingFactoryMethod(companyFactory[i]).Named("company" + i));

            for (int i = 0; i < companyFactory.Length; i++)
                Container.Register(Component.For<BusinessOneDAO>().ImplementedBy<BusinessOneDAOSQLImpl>()
                    .DependsOn(Dependency.OnComponent(typeof(SAPbobsCOM.Company), "company" + i)).Named("b1dao" + i));
            Container.Register(Component.For<BusinessOneUIDAO>().ImplementedBy<BusinessOneUIDAOImpl>());

            string runningFolder = Path.GetDirectoryName( Assembly.GetEntryAssembly().Location );

            Container.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().Pick()
                .WithService.DefaultInterfaces().LifestyleSingleton());
            Container.AddFacility<LoggingFacility>(f => f.UseLog4Net(Assembly.GetEntryAssembly().GetName().Name + ".config"));

            var logger = Container.Resolve<ILogger>();
            logger.Debug(String.Format(Messages.StartupFolder, runningFolder));
            SAPServiceFactory.Logger = logger;

            var b1dao = Container.Resolve<BusinessOneDAO>();
            SAPAppender.B1DAO = b1dao;

            return Container;
        }

   }
}
