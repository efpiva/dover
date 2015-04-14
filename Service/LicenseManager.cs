/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Model;
using Dover.Framework.DAO;
using Castle.Core.Logging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using Dover.Framework.Model.License;
using Dover.Framework.Monad;
using Dover.Framework.Attribute;
using Dover.Framework.Interface;
using Dover.Framework.Factory;

namespace Dover.Framework.Service
{

    internal class LicenseVerifyAddin : MarshalByRefObject
    {
        private LicenseDAO licenseDAO;
        private SAPbouiCOM.Application sapApp;

        public LicenseVerifyAddin(LicenseDAO licenseDAO, SAPbouiCOM.Application sapApp)
        {
            this.licenseDAO = licenseDAO;
            this.sapApp = sapApp;
        }

        internal bool AddinIsValid(string addin, out DateTime dueDate)
        {
            string xmlKey;
            string licenseXml;
            dueDate = DateTime.MinValue;
            // TOOD: colocar appdomain com addin.
            AddInAttribute addinAttribute = getAddinAttribute(addin);
            if (string.IsNullOrWhiteSpace(addinAttribute.LicenseFile))
            {
                dueDate = DateTime.MaxValue;
                return true;
            }
            xmlKey = readKeyXML(addin, addinAttribute.LicenseFile);

            if (string.IsNullOrWhiteSpace(xmlKey) || string.IsNullOrWhiteSpace(addinAttribute.Namespace))
                return false;

            licenseXml = licenseDAO.GetLicense(addinAttribute.Namespace);

            if (licenseXml == null || !CheckSignature(licenseXml, xmlKey))
                return false;

            string sysNumber = sapApp.Company.SystemId;
            string installNumber = sapApp.Company.InstallationId;

            LicenseHeader licenseFile = licenseXml.Deserialize<LicenseHeader>();

            if (!string.IsNullOrEmpty(licenseFile.SystemNumber) &&
                !string.IsNullOrEmpty(licenseFile.InstallNumber) &&
                licenseFile.SystemNumber == sysNumber && licenseFile.InstallNumber == installNumber)
            {
                List<LicenseModule> licenseModules = licenseFile.Items.Where(i => i.Name == addin).ToList(); ;
                if (licenseModules != null && licenseModules.Count() > 0)
                {
                    dueDate = licenseModules[0].ExpirationDate;
                }
            }

            return GetDate() < dueDate;
        }
        
        private AddInAttribute getAddinAttribute(string addin)
        {
            Assembly asm = AppDomain.CurrentDomain.Load(addin);
            if (asm == null)
                throw new ArgumentException("License public key not found");


            foreach (var type in asm.GetTypes())
            {
                object[] custAttr = type.GetCustomAttributes(typeof(AddInAttribute), true);
                if (custAttr.Count() > 0)
                {
                    return (AddInAttribute)custAttr[0];
                }
            }
            return null;
        }

        private DateTime GetDate()
        {
            return licenseDAO.GetDate();
        }

        private bool CheckSignature(string xml, string publicKey)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(xml);

            SignedXml signedXml = new SignedXml(xmlDoc);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");

            signedXml.LoadXml((XmlElement)nodeList[0]);
            var keyRSA = new RSACryptoServiceProvider();
            keyRSA.FromXmlString(publicKey);

            if (publicKey != null)
                return signedXml.CheckSignature(keyRSA);

            return false;
        }

        private string readKeyXML(string addin, string licenseFileResource)
        {
            Assembly asm = AppDomain.CurrentDomain.Load(addin);
            if (asm == null)
                throw new ArgumentException("License public key not found");

            using (var xmlresource = asm.GetManifestResourceStream(licenseFileResource))
            {
                if (xmlresource == null)
                    throw new ArgumentException("Resource not found");

                using (StreamReader sr = new StreamReader(xmlresource))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }

    /// <summary>
    /// Stub class
    /// </summary>
    [Transaction]
    internal class LicenseManager : MarshalByRefObject
    {
        private LicenseDAO licenseDAO;
        private FileUpdate fileUpdate;
        private AssemblyDAO asmDAO;

        public ILogger Logger { get; set; }

        public LicenseManager(AssemblyDAO asmDAO, FileUpdate fileUpdate,
            LicenseDAO licenseDAO)
        {
            this.licenseDAO = licenseDAO;
            this.asmDAO = asmDAO;
            this.fileUpdate = fileUpdate;
        }

        internal List<AssemblyInformation> ListAllAddins()
        {
            return asmDAO.GetAllAssembliesInformation(AssemblyType.Addin);
        }

        internal List<AssemblyInformation> ListAddins()
        {
            return asmDAO.GetAssembliesInformation(AssemblyType.Addin);
        }

        [Transaction]
        protected internal virtual void SaveLicense(string xmlPath)
        {
            string xml = null;
            if (File.Exists(xmlPath))
            {
                xml = File.ReadAllText(xmlPath);
            }
            else
            {
                throw new ArgumentException();
            }

            LicenseHeader licenseFile = xml.Deserialize<LicenseHeader>();
            string licenseID = licenseDAO.SaveLicense(xml, licenseFile.LicenseNamespace);

            UpdateAddinDueDateByNamespace(licenseFile.LicenseNamespace);
        }

        private void UpdateAddinDueDateByNamespace(string licenseNamespace)
        {
            List<string> addins = licenseDAO.getAddinsByNamespace(licenseNamespace);
            licenseDAO.UpdateNamespaceDueDate(licenseNamespace, DateTime.MinValue);

            var setup = new AppDomainSetup();
            setup.ApplicationName = "Dover.ConfigureDomain";
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            AppDomain configureDomain = AppDomain.CreateDomain("ConfigureDomain", null, setup);
            try
            {
                configureDomain.SetData("assemblyName", "tempDomain");
                IApplication app = (IApplication)configureDomain.CreateInstanceAndUnwrap("Framework",
                    "Dover.Framework.Application");
                SAPServiceFactory.PrepareForInception(configureDomain);
                LicenseManager licenseManager = app.Resolve<LicenseManager>();
                foreach (var addin in addins)
                {
                    DateTime dueDate;
                    if (licenseManager.AddinIsValid(addin, out dueDate))
                    {
                        licenseDAO.UpdateAddinDueDate(addin, dueDate);
                    }
                }
            }
            finally
            {
                AppDomain.Unload(configureDomain);
            }
        }

        internal void UpdateAddinDueDate(string addin)
        {
            if (addin == "Framework")
            {
                licenseDAO.UpdateAddinDueDate(addin, DateTime.MaxValue);
            }
            else
            {
                DateTime dueDate;
                if (AddinIsValid(addin, out dueDate))
                {
                    licenseDAO.UpdateAddinDueDate(addin, dueDate);
                }
            }
        }

        internal DateTime GetAddinDueDate(string module)
        {
            return licenseDAO.GetAddInDueDate(module);
        }

        internal bool AddinIsValid(string addin, out DateTime dueDate)
        {
            dueDate = DateTime.MinValue;

            var setup = new AppDomainSetup();
            setup.ApplicationName = "Dover.ConfigureDomain";
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            setup.ApplicationBase = tempDirectory;
            AppDomain configureDomain = AppDomain.CreateDomain("ConfigureDomain", null, setup);
            try
            {
                AssemblyInformation asm = asmDAO.GetAssemblyInformation(addin, AssemblyType.Addin);
                fileUpdate.UpdateAppDataFolder(asm, tempDirectory);

                configureDomain.SetData("assemblyName", "tempDomain");
                IApplication app = (IApplication)configureDomain.CreateInstanceAndUnwrap("Framework",
                    "Dover.Framework.Application");
                SAPServiceFactory.PrepareForInception(configureDomain);
                LicenseVerifyAddin licenseManager = app.Resolve<LicenseVerifyAddin>();

                if (asm != null)
                {
                    return licenseManager.AddinIsValid(addin, out dueDate);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Unhandled error", e);
            }
            finally
            {
                AppDomain.Unload(configureDomain);
                try
                {
                    Directory.Delete(tempDirectory, true);
                }
                catch (Exception e)
                {
                    Logger.Debug(string.Format("Directory {0} not cleaned", tempDirectory), e);
                }
            }
            return false;
        }
    }
}
