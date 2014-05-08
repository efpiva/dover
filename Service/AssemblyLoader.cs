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

namespace AddOne.Framework.Service
{
    internal enum AssemblySource
    {
        Core,
        AddIn
    }

    public class AssemblyLoader
    {
        private string[] addinsAssemblies = {
            "addInSetup.exe"
        };

        private string[] coreAssemblies = {
            "SAPbouiCOM.dll",
            "Framework.dll",
            "log4net.dll",
            "Castle.Core.dll",
            "Castle.Facilities.Logging.dll",
            "Castle.Services.Logging.Log4netIntegration.dll",
            "Castle.Windsor.dll",
            "AddOne.exe"
        };
        private AssemblyDAO asmDAO;
        public ILogger Logger { get; set; }


        public AssemblyLoader(AssemblyDAO asmDAO)
        {
            this.asmDAO = asmDAO;
        }

        public void RemoveAddIn(string moduleName)
        {
            // TODO: reload appDomain!
            asmDAO.RemoveAsm(moduleName);
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
                    SaveIfNotExistsOrDifferent(null, path.Substring(0, path.Length - 3), path, "A");
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
                // TODO: checar md5sum de B1s
                if (IsDifferent(asm, appFolder, fullPath))
                {
                    UpdateAssembly(asm, fullPath);
                    if (assemblyLocation == AssemblySource.AddIn)
                        UpdateB1StudioResource(appFolder, asm);
                }
            }
        }

        private void UpdateB1StudioResource(string appFolder, AssemblyInformation asmMeta)
        {
            try
            {
                if (asmMeta.ResourceName == null)
                {
                    Logger.Debug(String.Format(Messages.B1SResourceMissing, asmMeta.Name));
                    return;
                }
                string fullPath = Path.Combine(appFolder, asmMeta.ResourceName);
                var b1resource = asmDAO.GetB1StudioResource(asmMeta);
                if (b1resource != null)
                {
                    File.WriteAllBytes(fullPath, b1resource);
                    Logger.Info(String.Format(Messages.FileUpdated, asmMeta.ResourceName, asmMeta.Version));
                }
                else
                {
                    Logger.Warn(String.Format(Messages.FileMissing, asmMeta.ResourceName, asmMeta.Version));
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.FileError, asmMeta.ResourceName, asmMeta.Version), e);
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
            if (asms.Count > 0)
            {

                foreach (var asm in asms)
                {
                    ret.Add(SaveIfNotExistsOrDifferent(asm, asm.Name, asm.FileName, type));
                }
            }
            else
            {
                foreach (var asmFile in defaultAsms)
                {
                    ret.Add(SaveIfNotExistsOrDifferent(null, asmFile.Substring(0, asmFile.Length - 4), asmFile, type));
                }

            }
            return ret;

        }

        private AssemblyInformation SaveIfNotExistsOrDifferent(AssemblyInformation existingAsm, 
            string name, string asmFile, string type)
        {

            AssemblyInformation newAsm = new AssemblyInformation();
            if (existingAsm != null)
                newAsm.Code = existingAsm.Code; // Prepare for update.
            var asmPath = Path.Combine(Environment.CurrentDirectory, asmFile);
            newAsm.Name = name;
            newAsm.FileName = asmFile;
            byte[] asmBytes = File.ReadAllBytes(asmPath);
            newAsm.Version = GetFileVersion(asmBytes);
            newAsm.MD5 = MD5Sum(asmBytes);
            newAsm.Size = asmBytes.Length;
            newAsm.Date = DateTime.Now;
            newAsm.Type = type;

            if (existingAsm == null || newAsm.Version.CompareTo(existingAsm.Version) == 1
                || (newAsm.Version == existingAsm.Version && newAsm.MD5 != existingAsm.MD5))
            {
                var resourceName = asmFile.Substring(0, asmFile.Length-3) + "b1s";
                var b1sPath = Path.Combine(Environment.CurrentDirectory, resourceName);
                byte[] resource = null;
                if (File.Exists(b1sPath))
                {
                    resource = File.ReadAllBytes(b1sPath);
                    newAsm.ResourceName = resourceName;
                }

                asmDAO.SaveAssembly(newAsm, asmBytes, resource);
                return newAsm;
            }
            else
            {
                return existingAsm;
            }

        }

        private string GetFileVersion(byte[] asmBytes)
        {
            Assembly asm = AppDomain.CurrentDomain.Load(asmBytes);
            var version = asm.GetName().Version;
            return version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString()
                    + "." + version.Revision;
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
