using System;
using Castle.Windsor;
using AddOne.Framework.Factory;
using System.Threading;
using AddOne.Framework.Remoting;

namespace AddOne.Framework
{
    public class B1Application : MarshalByRefObject
    {
        private IWindsorContainer appContainer;

        public B1Application()
        {
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
