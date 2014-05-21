using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Factory;
using System.Threading;

namespace AddOne.Framework
{
    public class MicroBoot
    {
        internal string AppFolder;
        internal AppDomain Inception;

        internal void StartInception()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "AddOne.Inception";
            setup.ApplicationBase = AppFolder;
            Inception = AppDomain.CreateDomain("AddOne.Inception", null, setup);
            Environment.CurrentDirectory = AppFolder;
            SAPServiceFactory.PrepareForInception(Inception);
        }

        internal void Boot()
        {
            var thread = new Thread(new ThreadStart(this.PrivateBoot));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void PrivateBoot()
        {
            Inception.ExecuteAssembly("AddOne.exe", Environment.GetCommandLineArgs());
        }
    }
}
