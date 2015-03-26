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

namespace Dover.Framework.Service
{
    /// <summary>
    /// Stub class
    /// </summary>
    public class LicenseManager
    {
        private LicenseDAO licenseDAO;
        private AssemblyDAO asmDAO;
        public ILogger Logger { get; set; }

        // Change here your token, if you want to check for it. Place null if you want to ignore token check.
        byte[] clrToken = new byte[] { 163, 72, 246, 186, 128, 207, 208, 148 };
        private const string licensePath = "Dover.Framework.publicKey.xml";

        public LicenseManager(AssemblyDAO asmDAO, LicenseDAO licenseDAO)
        {
            this.licenseDAO = licenseDAO;
            this.asmDAO = asmDAO;
        }

        internal bool HasLicense()
        {
            return GetPublicKey() != null;
        }

        internal List<AssemblyInformation> ListAllAddins()
        {
            var retValue = asmDAO.GetAssembliesInformation(AssemblyType.Addin);
            foreach (var asm in retValue)
            {
                asm.ExpireDate = DateTime.MinValue;
            }
            return retValue;
        }

        internal List<AssemblyInformation> ListAddins()
        {
            var retValue = asmDAO.GetAssembliesInformation(AssemblyType.Addin);
            List<AssemblyInformation> filteredReturn = new List<AssemblyInformation>();

            string publicKey = GetPublicKey();
            if (publicKey == null)
            {
                filteredReturn = retValue;
            }
            else
            {
                try
                {
                    string xml = licenseDAO.GetLicense();
                    if (xml != null && CheckSignature(xml, publicKey))
                    {
                        License license = xml.Deserialize<License>();
                        Dictionary<string, DateTime> dueDates = getDueDates(license.Items);

                        foreach (var asm in retValue)
                        {
                            DateTime dueDate;
                            dueDates.TryGetValue(asm.Name, out dueDate);

                            if (dueDate < GetDate())
                            {
                                Logger.Error(string.Format(Messages.NoLicenseError, asm.Name));
                            }
                            else
                            {
                                filteredReturn.Add(asm);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(Messages.InvalidLicense);
                    Logger.Debug(Messages.InvalidLicense, e);
                }
            }

            return filteredReturn;
        }

        private Dictionary<string, DateTime> getDueDates(List<LicenseModule> list)
        {
            Dictionary<string, DateTime> ret = new Dictionary<string, DateTime>();
            foreach (var l in list)
            {
                ret.Add(l.Name, l.ExpirationDate);
            }
            return ret;
        }

        internal bool SaveLicense(string xmlPath)
        {
            string xml = null;
            if (File.Exists(xmlPath))
            {
                xml = File.ReadAllText(xmlPath);
            }
            else
            {
                return false;
            }

            string publicKey = GetPublicKey();
            if (publicKey == null)
            {
                throw new ArgumentException(); // shoulnd't be here. No license == no Manage License Menu.
            }

            if (CheckSignature(xml, publicKey))
            {
                licenseDAO.SaveLicense(xml);
                return true;
            }

            return false;
        }

        internal DateTime GetAddInExpireDate(string module)
        {
            string publicKey = GetPublicKey();
            if (publicKey == null)
            {
                return DateTime.MaxValue; // no license control.
            }

            string xml = licenseDAO.GetLicense();

            if (xml == null || !CheckSignature(xml, publicKey))
            {
                return DateTime.MinValue; // no license found, but license control enabled.
            }

            License licenseFile = xml.Deserialize<License>();
            List<LicenseModule> licenseModules = licenseFile.Items.Where(i => i.Name == module).ToList(); ;
            LicenseModule licenseModule = null;
            if (licenseModules != null && licenseModules.Count() > 0)
            {
                licenseModule = licenseModules.First();
            }

            if (licenseModule != null)
            {
                return licenseModule.ExpirationDate;
            }

            return DateTime.MinValue;
        }

        internal void AddInValid(string asm, out bool isValid, out bool hasLicense)
        {
            isValid = CheckAssembly(asm, clrToken);
            hasLicense = GetDate() < GetAddInExpireDate(asm);
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

        private string GetPublicKey()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(licensePath))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream)) 
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }

        private static bool CheckToken(string assembly, byte[] expectedToken)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (expectedToken == null)
                throw new ArgumentNullException("expectedToken");

            try
            {
                // Get the public static key token of the given assembly 
                Assembly asm = Assembly.LoadFrom(assembly);
                byte[] asmToken = asm.GetName().GetPublicKeyToken();

                // Compare it to the given token
                if (asmToken.Length != expectedToken.Length)
                    return false;

                for (int i = 0; i < asmToken.Length; i++)
                    if (asmToken[i] != expectedToken[i])
                        return false;

                return true;
            }
            catch (System.IO.FileNotFoundException)
            {
                // couldn't find the assembly
                return false;
            }
            catch (BadImageFormatException)
            {
                // the given file couldn't get through the loader
                return false;
            }
        }

        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern bool StrongNameSignatureVerificationEx(string wszFilePath, bool fForceVerification, ref bool pfWasVerified);

        private bool CheckAssembly(string asmName, byte[] clrToken)
        {
            bool notForced = false;
            if (clrToken == null || string.IsNullOrWhiteSpace(asmName)) // Token if for dummies.
                return true;

            string asmFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "addin", asmName, asmName + ".dll");

            bool verified = StrongNameSignatureVerificationEx(asmFullPath, false, ref notForced);

            if (verified && notForced)
            {
                bool isOK = CheckToken(asmFullPath, clrToken);

                return isOK && verified && notForced;
            }
            return false;
        }

    }
}
