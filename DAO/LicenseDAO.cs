using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.DAO
{
    public abstract class LicenseDAO
    {
        public abstract string ReadLicense();

        public abstract void SaveLicense(string xml);

        public abstract string GetSystemID();

        public abstract string GetInstallationID();

        public abstract DateTime GetServerDate();

    }
}
