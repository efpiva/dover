using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Dover.Framework.DAO;
using Castle.Facilities.Logging;
using Castle.Core.Logging;
using Dover.Framework.Log;
using System.Reflection;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using System.IO;
using Dover.Framework.Service;
using Castle.DynamicProxy;
using Dover.Framework.Proxy;
using Dover.Framework.Form;

namespace Dover.Framework.Factory
{

    public class ContainerManager
    {
        public static Func<SAPbobsCOM.Company>[] customCompanyFactory {get; set; }
        public static IWindsorContainer Container { get; private set; }

        internal static void RegisterAssembly(Assembly addIn)
        {
            Container.Register(Component.For<IInterceptor>().ImplementedBy<FormProxy>().Named("formProxy"));
            Container.Register(Classes.FromAssembly(addIn)
                            .IncludeNonPublicTypes().Pick()
                            .WithService.DefaultInterfaces().LifestyleTransient());
        }

        internal static IWindsorContainer BuildContainer()
        {
            string assemblyName;

            Func<SAPbobsCOM.Company>[] companyFactory =  (customCompanyFactory == null) 
                ? new Func<SAPbobsCOM.Company>[] {SAPServiceFactory.CompanyFactory} : customCompanyFactory;

            Container = new WindsorContainer();
            Container.Kernel.Resolver.AddSubResolver(new ArrayResolver(Container.Kernel));
            Container.Register(Component.For<LicenseDAO>().ImplementedBy<LicenseDAOSQLImpl>());

            Container.Register(Component.For<SAPbouiCOM.Application>().UsingFactoryMethod(SAPServiceFactory.ApplicationFactory));

            for(int i=0 ; i< companyFactory.Length ; i++)
                Container.Register(Component.For<SAPbobsCOM.Company>().UsingFactoryMethod(companyFactory[i]).Named("company" + i));

            for (int i = 0; i < companyFactory.Length; i++)
                Container.Register(Component.For<BusinessOneDAO>().ImplementedBy<BusinessOneDAOSQLImpl>()
                    .DependsOn(Dependency.OnComponent(typeof(SAPbobsCOM.Company), "company" + i)).Named("b1dao" + i));
            Container.Register(Component.For<BusinessOneUIDAO>().ImplementedBy<BusinessOneUIDAOImpl>());

            string runningFolder = Path.GetDirectoryName( AppDomain.CurrentDomain.BaseDirectory );

            Container.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().Pick()
                .WithService.DefaultInterfaces().LifestyleSingleton());

            assemblyName = Assembly.GetEntryAssembly() == null ? (string)AppDomain.CurrentDomain.GetData("assemblyName") 
                : Assembly.GetEntryAssembly().GetName().Name;

            Container.AddFacility<LoggingFacility>(f => f.UseLog4Net(assemblyName + ".config"));

            var logger = Container.Resolve<ILogger>();
            logger.Debug(DebugString.Format(Messages.StartupFolder, runningFolder));
            SAPServiceFactory.Logger = logger;

            var b1dao = Container.Resolve<BusinessOneDAO>();
            SAPAppender.B1DAO = b1dao;

            return Container;
        }

   }
}
