using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.Core.Logging;
using AddOne.Framework.Service;

namespace AddOne.Framework
{
    public class Boot : MarshalByRefObject
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
            List<string> addins = licenseManager.ListAddins();
            addinLoader.LoadAddins(addins);
        }
    }
}
