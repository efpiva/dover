using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.DAO
{
    public abstract class LicenseDAO
    {
        internal abstract string ReadLicense();

        internal abstract void SaveLicense(string xml);

        internal abstract string GetSystemID();

        internal abstract string GetInstallationID();

        internal abstract DateTime GetServerDate();

    }
}
