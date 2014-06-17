using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;
using AddOne.Framework.Model;
using AddOne.Framework.Monad;
using System.Security.Cryptography;
using System.IO;
using Castle.Core.Logging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Reflection;
using AddOne.Framework.Attribute;

namespace AddOne.Framework.Service
{
    internal enum AssemblySource
    {
        Core,
        AddIn
    }

    public class AssemblyManager
    {
        private string[] addinsAssemblies = {
            "addInSetup.dll"
        };

        private string[] coreAssemblies = {
            "SAPbouiCOM.dll",
            "Interop.SAPbobsCOM.dll",
            "Framework.dll",
            "log4net.dll",
            "Castle.Core.dll",
            "Castle.Facilities.Logging.dll",
            "Castle.Services.Logging.Log4netIntegration.dll",
            "Castle.Windsor.dll",
            "AddOne.exe"
        };
        private AssemblyDAO asmDAO;
        private LicenseManager licenseManager;
        private AddIni18n addIni18n;
        public ILogger Logger { get; set; }


        public AssemblyManager(AssemblyDAO asmDAO, LicenseManager licenseManager, AddIni18n addIni18n)
        {
            this.asmDAO = asmDAO;
            this.licenseManager = licenseManager;
            this.addIni18n = addIni18n;
        }

        public void RemoveAddIn(string moduleName)
        {
            // TODO: reload appDomain!
            asmDAO.RemoveAsm(moduleName);
            Logger.Info(string.Format(Messages.RemoveAddinSuccess, moduleName));
        }

        public void SaveAddIn(string path)
        {
            if (path == null || path.Length < 4)
            {
                Logger.Error(string.Format(Messages.SaveAddInError, path.Return( x => x, String.Empty)));
            }
            else
            {
                try
                {
                    // TODO: check CustomAttributes
                    var fileName = Path.GetFileName(path);
                    var addInName = fileName.Substring(0, fileName.Length - 4);
                    var existingAsm = asmDAO.GetAddInAssembly(addInName);
                    SaveIfNotExistsOrDifferent(existingAsm, addInName, fileName, 
                        Path.GetDirectoryName(path), "A");
                    licenseManager.BootLicense(); // reload licenses to include added license.
                    Logger.Info(string.Format(Messages.SaveAddInSuccess, path));
                }
                catch (Exception e)
                {
                    Logger.Error(string.Format(Messages.SaveAddInError, path), e);
                }
            }

        }

        internal void UpdateAssemblies(AssemblySource assemblyLocation, string appFolder)
        {
            List<AssemblyInformation> asms;
            Logger.Debug(String.Format(Messages.UpdatingAssembly, assemblyLocation));
            if (assemblyLocation == AssemblySource.Core)
            {
                asms = InitializeCoreAssemblies(appFolder);
            }
            else
                asms = InitializeAddInAssemblies(appFolder);

            foreach(var asm in asms)
            {
                string fullPath = Path.Combine(appFolder, asm.FileName);
                if (IsDifferent(asm, appFolder, fullPath))
                {
                    UpdateAssembly(asm, fullPath);
                }
            }
        }

        private List<AssemblyInformation> InitializeAddInAssemblies(string appFolder)
        {
            List<AssemblyInformation> addinsAsms = asmDAO.GetAddinsAssemblies();

            return GenericInitialize(addinsAsms, "A", addinsAssemblies);
        }

        private List<AssemblyInformation> InitializeCoreAssemblies(string appFolder)
        {
            List<AssemblyInformation> coreAsms = asmDAO.GetCoreAssemblies();

            return GenericInitialize(coreAsms, "C", coreAssemblies);
        }

        private List<AssemblyInformation> GenericInitialize(List<AssemblyInformation> asms, string type,
            string[] defaultAsms)
        {
            List<AssemblyInformation> ret = new List<AssemblyInformation>();
            // load databaseAddins.
            if (asms.Count > 0 && defaultAsms.Length <= asms.Count)
            {

                foreach (var asm in asms)
                {
                    try
                    {
                        ret.Add(SaveIfNotExistsOrDifferent(asm, asm.Name, asm.FileName, Environment.CurrentDirectory, type));
                    }
                    catch (FileNotFoundException)
                    {
                        ret.Add(asm);
                        Logger.Warn(String.Format(Messages.IgnoringFile, asm.FileName));
                    }
                }
            }
            else
            {
                foreach (var asmFile in defaultAsms)
                {
                    try
                    {
                        AssemblyInformation asm = (type == "A") ? asmDAO.GetAddInAssembly(asmFile.Substring(0, asmFile.Length - 4))
                            : asmDAO.GetCoreAssembly(asmFile.Substring(0, asmFile.Length - 4));
                        ret.Add(SaveIfNotExistsOrDifferent(asm, asmFile.Substring(0, asmFile.Length - 4), asmFile, Environment.CurrentDirectory, type));
                    }
                    catch (FileNotFoundException)
                    {
                        Logger.Warn(String.Format(Messages.IgnoringFile, asmFile));
                    }
                }

            }
            return ret;

        }

        private AssemblyInformation SaveIfNotExistsOrDifferent(AssemblyInformation existingAsm, 
            string name, string asmFile, string path, string type)
        {

            AssemblyInformation newAsm = new AssemblyInformation();
            if (existingAsm != null)
                newAsm.Code = existingAsm.Code; // Prepare for update.
            var asmPath = Path.Combine(path, asmFile);
            newAsm.Name = name;
            newAsm.FileName = asmFile;
            byte[] asmBytes = File.ReadAllBytes(asmPath);
            GetAssemblyInfoFromBin(asmBytes, newAsm);
            newAsm.MD5 = MD5Sum(asmBytes);
            newAsm.Size = asmBytes.Length;
            newAsm.Date = DateTime.Now;
            newAsm.Type = type;

            if (existingAsm == null || newAsm.Version.CompareTo(existingAsm.Version) == 1
                || (newAsm.Version == existingAsm.Version && newAsm.MD5 != existingAsm.MD5))
            {
                asmDAO.SaveAssembly(newAsm, asmBytes);
                return newAsm;
            }
            else
            {
                return existingAsm;
            }

        }

        private void GetAssemblyInfoFromBin(byte[] asmBytes, AssemblyInformation asmInfo)
        {
            bool found = false;
            Assembly asm = AppDomain.CurrentDomain.Load(asmBytes);
            var version = asm.GetName().Version;
            asmInfo.Version = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString()
                    + "." + version.Revision;

            var types = (from type in asm.GetTypes()
                        where type.IsClass
                        select type);

            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    if (attr is AddInAttribute)
                    {
                        var addInAttribute = ((AddInAttribute)attr);
                        if (!string.IsNullOrEmpty(addInAttribute.i18n))
                        {
                            asmInfo.Description = addIni18n.GetLocalizedString(addInAttribute.i18n, asm);
                        }
                        else
                        {
                            asmInfo.Description = ((AddInAttribute)attr).Description;
                        }
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }

        }

        private void UpdateAssembly(AssemblyInformation asmMeta, string fullPath)
        {
            try
            {
                byte[] asm = asmDAO.GetAssembly(asmMeta);
                if (asm != null)
                {
                    File.WriteAllBytes(fullPath, asm);
                    Logger.Info(String.Format(Messages.FileUpdated, asmMeta.Name, asmMeta.Version));
                }
                else
                {
                    Logger.Warn(String.Format(Messages.FileMissing, asmMeta.Name, asmMeta.Version));
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.FileError, asmMeta.Name, asmMeta.Version), e);
            }
        }

        private bool IsDifferent(AssemblyInformation asm, string appFolder, string fullPath)
        {
            return !File.Exists(fullPath) || !CheckSum(asm.MD5, fullPath);
        }

        private string MD5Sum(byte[] fileByte)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(fileByte);
                var sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                var md5sum = sb.ToString();
                Logger.Debug(String.Format(Messages.MD5Sum, md5sum));
                return md5sum;
            }
        }

        private bool CheckSum(string asmHash, string filename)
        {
            Logger.Debug(String.Format(Messages.CheckSum, asmHash, filename));
            byte[] fileByte = File.ReadAllBytes(filename);
            return MD5Sum(fileByte) == asmHash;
        }

    }
}
