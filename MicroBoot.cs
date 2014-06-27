using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Factory;
using System.Threading;
using AddOne.Framework.Remoting;
using AddOne.Framework.Service;

namespace AddOne.Framework
{
    public class MicroBoot
    {
        internal string AppFolder;
        internal AppDomain Inception;
        private I18NService I18NService;

        public MicroBoot(I18NService I18NService)
        {
            this.I18NService = I18NService;
        }

        internal void StartInception()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "AddOne.Inception";
            setup.ApplicationBase = AppFolder;
            Inception = AppDomain.CreateDomain("AddOne.Inception", null, setup);
            Environment.CurrentDirectory = AppFolder;
        }

        internal void Boot()
        {
            var thread = new Thread(new ThreadStart(this.PrivateBoot));
            thread.SetApartmentState(ApartmentState.STA);
            I18NService.ConfigureThreadI18n(thread);
            thread.Start();
        }

        private void PrivateBoot()
        {
            B1Application app = (B1Application)Inception.CreateInstanceAndUnwrap("Framework", "AddOne.Framework.B1Application");
            SAPServiceFactory.PrepareForInception(Inception); // need to be after B1Application creation because of assembly resolving from embedded resources.
            Inception.SetData("assemblyName", "AddOne"); // Used to get current AssemblyName for logging and reflection
            Sponsor<B1Application> appSponsor = new Sponsor<B1Application>(app);
            app.Run();
        }
    }
}
