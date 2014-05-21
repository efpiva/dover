using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.Core.Logging;
using AddOne.Framework.Service;

namespace AddOne.Framework
{
    public class Boot
    {
        public ILogger Logger { get; set; }

        private LicenseManager licenseManager;
        private AddinLoader addinLoader;
        private EventDispatcher dispatcher;
        private SAPbouiCOM.Framework.Application app;

        public Boot(LicenseManager licenseValidation, AddinLoader addinLoader, EventDispatcher dispatcher,
            SAPbouiCOM.Framework.Application app)
        {
            this.licenseManager = licenseValidation;
            this.addinLoader = addinLoader;
            this.dispatcher = dispatcher;
            this.app = app;
        }

        public void StartUp()
        {
            try
            {
                Logger.Info(String.Format(Messages.Starting, this.GetType().Assembly.GetName().Version));
                var addins = licenseManager.ListAddins();
                addinLoader.LoadAddins(addins);
                dispatcher.RegisterEvents();
                addinLoader.CreateInceptionServer();
                app.Run();
            }
            catch (Exception e)
            {
                Logger.Fatal(Messages.ErrorStartup, e);
                Environment.Exit(10);
            }
        }


        public void StartThis()
        {
            try
            {
                Logger.Info(String.Format(Messages.Starting, this.GetType().Assembly.GetName().Version));
                addinLoader.StartThis();
                dispatcher.RegisterEvents();
            }
            catch (Exception e)
            {
                Logger.Fatal(Messages.ErrorStartup, e);
                Environment.Exit(10);
            }
        }
    }
}
