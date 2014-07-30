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

            string runningFolder = Path.GetDirectoryName( AppDomain.CurrentDomain.BaseDirectory );

            // AddinManager registration. If I'm an AddIn, get addinManager from AppDomain, so
            // both (addin AppDomain and inception AppDomain) references the same implementation.
            AddinManager addinManager = (AddinManager)AppDomain.CurrentDomain.GetData("frameworkManager");
            if (addinManager != null)
            {
                addinManagerSponsor = new Sponsor<AddinManager>(addinManager);
                Container.Register(Component.For<AddinManager>().Instance(addinManager));
            }

            // Service registration, they are singleton.
            Container.Register(Classes.FromThisAssembly().IncludeNonPublicTypes().InNamespace("Dover.Framework.Service")
                .WithService.DefaultInterfaces().LifestyleSingleton());
            // DAO registration. Abstract classes, they're singleton.
            Container.Register(Component.For<BusinessOneUIDAO>().ImplementedBy<BusinessOneUIDAOImpl>());
            Container.Register(Component.For<AssemblyDAO>().ImplementedBy<AssemblyDAOImpl>());
            Container.Register(Component.For<PermissionDAO>().ImplementedBy<PermissionDAOSQLImpl>());

            // Core and MicroCore
            Container.Register(Component.For<MicroCore>().LifestyleSingleton());
            Container.Register(Component.For<MicroBoot>().LifestyleSingleton());
            Container.Register(Component.For<Boot>().LifestyleSingleton());

            assemblyName = Assembly.GetEntryAssembly() == null ? (string)AppDomain.CurrentDomain.GetData("assemblyName") 
                : Assembly.GetEntryAssembly().GetName().Name;

            if (assemblyName == "Framework")
                assemblyName = "Dover"; // Framework should be threated the same as Dover.

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
