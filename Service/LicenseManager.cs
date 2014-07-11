using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Model;
using Dover.Framework.DAO;

namespace Dover.Framework.Service
{
    /// <summary>
    /// Stub class
    /// </summary>
    public class LicenseManager
    {
        private AssemblyDAO asmDAO;

        public LicenseManager(AssemblyDAO asmDAO)
        {
            this.asmDAO = asmDAO;
        }

        internal List<AssemblyInformation> ListAddins()
        {
            return asmDAO.getAssembliesInformation("A");
        }

        internal void BootLicense()
        {
            // fake implementation
        }

        public void SaveLicense(string p)
        {
            // fake implementation
        }

        public DateTime GetAddInExpireDate(string module)
        {
            return DateTime.MaxValue;
        }
    }
}
