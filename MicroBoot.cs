using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Factory;
using System.Threading;
using Dover.Framework.Remoting;
using Dover.Framework.Service;

namespace Dover.Framework
{
    public class MicroBoot
    {
        internal string AppFolder;
        private I18NService I18NService;

        internal AppDomain Inception { get; set; }
        internal AddinManager InceptionAddinManager { get; set; }

        public MicroBoot(I18NService I18NService)
        {
            this.I18NService = I18NService;
        }

        internal void StartInception()
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "Dover.Inception";
            setup.ApplicationBase = AppFolder;
            Inception = AppDomain.CreateDomain("Dover.Inception", null, setup);
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
            Application app = (Application)Inception.CreateInstanceAndUnwrap("Framework", "Dover.Framework.Application");
            SAPServiceFactory.PrepareForInception(Inception); // need to be after Application creation because of assembly resolving from embedded resources.
            Inception.SetData("assemblyName", "Dover"); // Used to get current AssemblyName for logging and reflection            
            InceptionAddinManager = app.Resolve<AddinManager>();
            Sponsor<Application> appSponsor = new Sponsor<Application>(app);
            Sponsor<AddinManager> addInManagerSponsor = new Sponsor<AddinManager>(InceptionAddinManager);
            app.RunInception();
        }
    }
}
