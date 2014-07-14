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
    /// <summary>
    /// Class that encapsulates creation of WindsorContainer, responsible for IoC of Dover framework.
    /// 
    /// </summary>
    public class ContainerManager
    {
        /// <summary>
        /// If you want to run Dover Framework as a windows service or as a command line tool, 
        /// one or more company Factors should be defined, since there will be no UI-API
        /// to do the connection using session authentication.
        /// </summary>
        public static Func<SAPbobsCOM.Company>[] customCompanyFactory {get; set; }
        /// <summary>
        /// WindsorContainer used by Dover. In some situations you may need to manipulate it directly.
        /// 
        /// Use with caution.
        /// </summary>
        public static IWindsorContainer Container { get; private set; }

        internal static void RegisterAssembly(Assembly addIn)
        {
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
            // proxy for forms.
            Container.Register(Component.For<IInterceptor>().ImplementedBy<FormProxy>().Named("formProxy"));
            // forms are Transient.
            Container.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().InNamespace("Dover.Framework.Form")
                .WithService.DefaultInterfaces().LifestyleTransient());

            // SAP App Facttory.
            Container.Register(Component.For<SAPbouiCOM.Application>().UsingFactoryMethod(SAPServiceFactory.ApplicationFactory));

            // company factory (use default, that initializes DI API from App if none was provided).
            for(int i=0 ; i< companyFactory.Length ; i++)
                Container.Register(Component.For<SAPbobsCOM.Company>().UsingFactoryMethod(companyFactory[i]).Named("company" + i));
            for (int i = 0; i < companyFactory.Length; i++)
                Container.Register(Component.For<BusinessOneDAO>().ImplementedBy<BusinessOneDAOSQLImpl>()
                    .DependsOn(Dependency.OnComponent(typeof(SAPbobsCOM.Company), "company" + i)).Named("b1dao" + i));
            Container.Register(Component.For<BusinessOneUIDAO>().ImplementedBy<BusinessOneUIDAOImpl>());

            string runningFolder = Path.GetDirectoryName( AppDomain.CurrentDomain.BaseDirectory );

            // Service and DAO registration, they are singleton.
            Container.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().InNamespace("Dover.Framework.Service")
                .WithService.DefaultInterfaces().LifestyleSingleton());
            Container.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().InNamespace("Dover.Framework.DAO")
                .WithService.DefaultInterfaces().LifestyleSingleton());

            // Core and MicroCore
            Container.Register(Component.For<MicroCore>().LifestyleSingleton());
            Container.Register(Component.For<MicroBoot>().LifestyleSingleton());
            Container.Register(Component.For<Boot>().LifestyleSingleton());

            assemblyName = Assembly.GetEntryAssembly() == null ? (string)AppDomain.CurrentDomain.GetData("assemblyName") 
                : Assembly.GetEntryAssembly().GetName().Name;

            if (assemblyName == "Framework")
                assemblyName = "Dover"; // Framework should be threated the same as Dover.
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
