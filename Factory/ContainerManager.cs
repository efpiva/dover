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
using Dover.Framework.Remoting;
using Castle.Core;
using Dover.Framework.Attribute;
using SAPbouiCOM;

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
        private static Sponsor<AddinManager> addinManagerSponsor;
        private static Sponsor<AppEventHandler> appEventHandlerSponsor;

        internal static void RegisterAssembly(Assembly addIn)
        {
            Container.Register(Classes.FromAssembly(addIn)
                            .IncludeNonPublicTypes().Pick()
                            .WithService.DefaultInterfaces().LifestyleTransient());
        }

        internal static void CheckProxy(Assembly addIn)
        {
            foreach (var type in addIn.GetTypes())
            {
                if (typeof(DoverFormBase).IsAssignableFrom(type))
                {
                    CheckFormType(type);
                }
                var interceptors = type.GetCustomAttributes(true);
                foreach (var obj in interceptors)
                {
                    TransactionAttribute interceptor = obj as TransactionAttribute;
                    if (interceptor != null)
                    {
                        CheckTransactionType(type);
                    }
                }
            }
        }

        private static void CheckFormType(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (IsEventMethod(method) && (method.IsPrivate || !method.IsVirtual))
                    throw new ArgumentException(string.Format(Messages.FormDeclarationError, method.Name, type.Name));
            }
        }

        private static bool IsEventMethod(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Count() == 2
                && parameters[0].ParameterType == typeof(Object)
                && parameters[1].ParameterType == typeof(SBOItemEventArg);
        }

        private static void CheckTransactionType(Type type)
        {
            var transactionAttr = type.GetCustomAttributes(typeof(TransactionAttribute), true);
            if (transactionAttr.Count() == 0)
                return;

            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                transactionAttr = method.GetCustomAttributes(typeof(TransactionAttribute), true);
                if (transactionAttr.Count() == 0)
                    continue;

                if (method.IsPrivate || !method.IsVirtual)
                {
                    throw new ArgumentException(string.Format(Messages.TransactionDeclarationError, method.Name, type.Name));
                }
            }
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
            // proxy for FormEvents
            Container.Register(Component.For<IInterceptor>().ImplementedBy<EventProxy>().Named("eventProxy"));
            // proxy for Transactions
            Container.Register(Component.For<IInterceptor>().ImplementedBy<TransactionProxy>().Named("transactionProxy"));
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

            string runningFolder = Path.GetDirectoryName( AppDomain.CurrentDomain.BaseDirectory );

            // AddinManager registration. If I'm an AddIn, get addinManager from AppDomain, so
            // both (addin AppDomain and inception AppDomain) references the same implementation.
            AddinManager addinManager = (AddinManager)AppDomain.CurrentDomain.GetData("frameworkManager");
            if (addinManager != null)
            {
                addinManagerSponsor = new Sponsor<AddinManager>(addinManager);
                Container.Register(Component.For<AddinManager>().Instance(addinManager));
            }

            AppEventHandler appEventHandler = (AppEventHandler)AppDomain.CurrentDomain.GetData("appHandler");
            if (appEventHandler != null)
            {
                appEventHandlerSponsor = new Sponsor<AppEventHandler>(appEventHandler);
                Container.Register(Component.For<AppEventHandler>().Instance(appEventHandler));
            }

            // Service registration, they are singleton.
            Container.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().InNamespace("Dover.Framework.Service")
                .WithService.DefaultInterfaces().LifestyleSingleton());
            // DAO registration. Abstract classes, they're singleton.
            Container.Register(Component.For<BusinessOneUIDAO>().ImplementedBy<BusinessOneUIDAOImpl>());
            Container.Register(Component.For<AssemblyDAO>().ImplementedBy<AssemblyDAOImpl>());
            Container.Register(Component.For<PermissionDAO>().ImplementedBy<PermissionDAOSQLImpl>());
            Container.Register(Component.For<LicenseDAO>().ImplementedBy<LicenseDAOImpl>());

            // Core and MicroCore
            Container.Register(Component.For<MicroCore>().LifestyleSingleton());
            Container.Register(Component.For<MicroBoot>().LifestyleSingleton());
            Container.Register(Component.For<Boot>().LifestyleSingleton());

            assemblyName = Assembly.GetEntryAssembly() == null ? (string)AppDomain.CurrentDomain.GetData("assemblyName") 
                : Assembly.GetEntryAssembly().GetName().Name;

            if (assemblyName == "Framework")
            {
                #if DEBUG
                CheckProxy(Assembly.Load(assemblyName)); // if passed on debug tests, we do not need this on production.
                #endif
                assemblyName = "Dover"; // Framework should be threated the same as Dover.
            }

            if (!File.Exists(assemblyName + ".config"))
                assemblyName = "DoverTemp"; // Temp AppDomain logging.

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
