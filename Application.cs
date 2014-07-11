using System;
using Castle.Windsor;
using Dover.Framework.Factory;
using System.Threading;
using Dover.Framework.Remoting;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace Dover.Framework
{
    public class Application : MarshalByRefObject
    {
        private IWindsorContainer appContainer;
        private static Dictionary<string, Assembly> assemblyCacheResolver = new Dictionary<string, Assembly>();
        private string[] embeddedAssemblies = {
            "Dover.Framework.Assemblies.SAPbouiCOM.dll",
            "Dover.Framework.Assemblies.Interop.SAPbobsCOM.dll",
            "Dover.Framework.Assemblies.log4net.dll",
            "Dover.Framework.Assemblies.Castle.Core.dll",
            "Dover.Framework.Assemblies.Castle.Facilities.Logging.dll",
            "Dover.Framework.Assemblies.Castle.Services.Logging.Log4netIntegration.dll",
            "Dover.Framework.Assemblies.Castle.Windsor.dll",
            "Dover.Framework.Assemblies.ICSharpCode.SharpZipLib.dll"};


        public Application()
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
            {
                appContainer = ContainerManager.BuildContainer();
            }

            return appContainer.ResolveAll<T>();
        }
        
        public T Resolve<T>()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }

            return appContainer.Resolve<T>();
        }

        public void Run()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }
            var microCore = appContainer.Resolve<MicroCore>();
            microCore.PrepareFramework();
        }

        public void RunAddin()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }
            var loader = appContainer.Resolve<Boot>();
            loader.StartThis();
            ManualResetEvent shutdownEvent = (ManualResetEvent)AppDomain.CurrentDomain.GetData("shutdownEvent");
            Sponsor<ManualResetEvent> shutdownEventSponsor = new Sponsor<ManualResetEvent>(shutdownEvent);
            shutdownEvent.WaitOne(); // Wait until shutdown event is signaled.
        }

        public void RunInception()
        {
            if (appContainer == null)
            {
                appContainer = ContainerManager.BuildContainer();
            }
            var boot = appContainer.Resolve<Boot>();
            boot.StartUp();
            appContainer.Dispose();
        }
    }
}
