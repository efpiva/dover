using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.Core.Logging;
using AddOne.Framework.Service;

namespace AddOne.Framework
{
    internal class Boot
    {
        public ILogger Logger { get; set; }

        private LicenseManager licenseManager;
        private AddinLoader addinLoader;

        public Boot(LicenseManager licenseValidation, AddinLoader addinLoader)
        {
            this.licenseManager = licenseValidation;
            this.addinLoader = addinLoader;
        }

        public void StartUp()
        {
            try
            {
                Logger.Info(String.Format(Messages.Starting, this.GetType().Assembly.GetName().Version));
                List<string> addins = licenseManager.ListAddins();
                addinLoader.LoadAddins(addins);
            }
            catch (Exception e)
            {
                Logger.Fatal(String.Format(Messages.ErrorStartup, e), e);
                Environment.Exit(10);
            }
        }
    }
}
