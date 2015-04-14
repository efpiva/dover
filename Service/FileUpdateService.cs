using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Model;
using System.IO;
using System.Security.Cryptography;
using Dover.Framework.Log;
using Castle.Core.Logging;
using Dover.Framework.DAO;

namespace Dover.Framework.Service
{
    internal class FileUpdate
    {
        private ILogger Logger;
        private AssemblyDAO asmDAO;
        private SAPbobsCOM.Company company;

        public FileUpdate(ILogger Logger, AssemblyDAO asmDAO, SAPbobsCOM.Company company)
        {
            this.Logger = Logger;
            this.asmDAO = asmDAO;
            this.company = company;
        }

        internal void UpdateAppDataFolder(AssemblyInformation asm, string appFolder)
        {
            string fullPath = Path.Combine(appFolder, asm.FileName);
            List<AssemblyInformation> dependencies = asmDAO.GetDependencies(asm);
            if (IsDifferent(asm, fullPath))
            {
                UpdateAssembly(asm, fullPath);
            }
            foreach (var dep in dependencies)
            {
                fullPath = Path.Combine(appFolder, dep.FileName);
                if (IsDifferent(dep, fullPath))
                {
                    UpdateAssembly(dep, fullPath);
                }
            }
            if (dependencies.Count == 0)
            {
                AssemblyInformation coreAsm = asmDAO.GetAssemblyInformation("Framework", AssemblyType.Core);
                UpdateAppDataFolder(coreAsm, appFolder);
            }
        }

        private bool IsDifferent(AssemblyInformation asm, string fullPath)
        {
            return !File.Exists(fullPath) || !CheckSum(asm.MD5, fullPath);
        }

        internal static string MD5Sum(byte[] fileByte)
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
                return md5sum;
            }
        }

        private bool CheckSum(string asmHash, string filename)
        {
            Logger.Debug(DebugString.Format(Messages.CheckSum, asmHash, filename));
            byte[] fileByte = File.ReadAllBytes(filename);
            return MD5Sum(fileByte) == asmHash;
        }

        private void UpdateAssembly(AssemblyInformation asmMeta, string fullPath)
        {
            try
            {
                Logger.Debug(String.Format(Messages.FileUpdating, asmMeta.Name, asmMeta.Version));
                string cacheFile = Path.Combine(GetDoverDirectory(), "..", "Cache", asmMeta.MD5);
                if (!CreateFromCache(asmMeta, cacheFile, fullPath))
                {
                    byte[] asmBytes = asmDAO.GetAssembly(asmMeta);
                    if (asmBytes != null)
                    {
                        File.WriteAllBytes(cacheFile, asmBytes);
                        CreateFromCache(asmMeta, cacheFile, fullPath);
                    }
                    else
                    {
                        Logger.Warn(String.Format(Messages.FileMissing, asmMeta.Name, asmMeta.Version));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.FileError, asmMeta.Name, asmMeta.Version), e);
            }
        }

        private bool CreateFromCache(AssemblyInformation asmMeta, string cacheFile, string fullPath)
        {
            if (File.Exists(cacheFile))
            {
                string baseDirectory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory);
                File.Copy(cacheFile, fullPath, true);
                Logger.Debug(String.Format(Messages.FileUpdated, asmMeta.Name, asmMeta.Version));
                return true;
            }
            return false;
        }

        internal string GetDoverDirectory()
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dover";

            string server = company.Server;
            int port = server.LastIndexOf(":");
            if (port > 0)
                server = server.Substring(0, port); // Hana servers have :
            appFolder = Path.Combine(appFolder, server + "-" + company.CompanyDB);
            return appFolder;
        }


    }
}
