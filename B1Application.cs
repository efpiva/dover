using System;
using Castle.Windsor;
using AddOne.Framework.Factory;
using System.Threading;
using AddOne.Framework.Remoting;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace AddOne.Framework
{
    public class B1Application : MarshalByRefObject
    {
        private IWindsorContainer appContainer;
        private static Dictionary<string, Assembly> assemblyCacheResolver = new Dictionary<string, Assembly>();
        private string[] embeddedAssemblies = {
            "AddOne.Framework.Assemblies.SAPbouiCOM.dll",
            "AddOne.Framework.Assemblies.Interop.SAPbobsCOM.dll",
            "AddOne.Framework.Assemblies.log4net.dll",
            "AddOne.Framework.Assemblies.Castle.Core.dll",
            "AddOne.Framework.Assemblies.Castle.Facilities.Logging.dll",
            "AddOne.Framework.Assemblies.Castle.Services.Logging.Log4netIntegration.dll",
            "AddOne.Framework.Assemblies.Castle.Windsor.dll",
            "AddOne.Framework.Assemblies.ICSharpCode.SharpZipLib.dll"};


        public B1Application()
        {
            // load all embedded resource into memory;
            byte[] ba = null;
            Assembly curAsm = Assembly.GetExecutingAssembly();
            foreach (string resource in embeddedAssemblies)
            {
                using (Stream stm = curAsm.GetManifestResourceStream(resource))
                {
                    ba = new byte[(int)stm.Length];
                    stm.Read(ba, 0, (int)stm.Length);
                    Assembly asm = Assembly.Load(ba);
                    assemblyCacheResolver.Add(asm.FullName, asm);
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (assemblyCacheResolver.ContainsKey(args.Name))
                return assemblyCacheResolver[args.Name];
            return null;
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
                microCore.PrepareFramework();
            }
            else if (AppDomain.CurrentDomain.FriendlyName == "AddOne.AddIn")
            {
                var container = ContainerManager.BuildContainer();
                var loader = container.Resolve<Boot>();
                loader.StartThis();
                ManualResetEvent shutdownEvent = (ManualResetEvent)AppDomain.CurrentDomain.GetData("shutdownEvent");
                Sponsor<ManualResetEvent> shutdownEventSponsor = new Sponsor<ManualResetEvent>(shutdownEvent);
                shutdownEvent.WaitOne(); // Wait until shutdown event is signaled.
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
