using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.Core.Logging;
using AddOne.Framework.Service;
using System.Windows.Forms;

namespace AddOne.Framework
{
    public class Boot
    {
        public ILogger Logger { get; set; }

        private LicenseManager licenseManager;
        private AddinManager addinLoader;
        private EventDispatcher dispatcher;

        public Boot(LicenseManager licenseValidation, AddinManager addinLoader, EventDispatcher dispatcher)
        {
            this.licenseManager = licenseValidation;
            this.addinLoader = addinLoader;
            this.dispatcher = dispatcher;
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
                Application.Run();
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
