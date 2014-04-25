using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;
using AddOne.Framework.Model.Assembly;
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
        private string[] coreAssemblies = {
            "Framework.dll",
            "log4net.dll",
            "Castle.Core.dll",
            "Castle.Facilities.Logging.dll",
            "Castle.Windsor.dll",
            "AddOne.exe"
        };
        private AssemblyDAO asmDAO;
        public ILogger Logger { get; set; }


        public AssemblyLoader(AssemblyDAO asmDAO)
        {
            this.asmDAO = asmDAO;
        }

        internal void UpdateAssemblies(AssemblySource assemblyLocation, string appFolder)
        {
            List<AssemblyInformation> asms;
            if (assemblyLocation == AssemblySource.Core)
            {
                asms = InitializeCoreAssemblies(appFolder);
            }
            else
                asms = asmDAO.GetAddinsAssemblies();

            foreach(var asm in asms)
            {
                string fullPath = Path.Combine(appFolder, asm.Name);
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
                string fullPath = Path.Combine(appFolder, asmMeta.ResourceName);
                string b1resource = asmDAO.GetB1StudioResource(asmMeta);
                if (b1resource != null)
                {
                    File.WriteAllText(fullPath, b1resource);
                    Logger.Info(String.Format("Atualizado arquivo {0} - Versao {1}", asmMeta.Name, asmMeta.Version));
                }
                else
                {
                    Logger.Warn(String.Format("Arquivo {0} - Versao {1} - não encontrado", asmMeta.Name, asmMeta.Version));
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format("Erro atualizando arquivo {0} - Versao {1}.", asmMeta.Name, asmMeta.Version), e);
            }
        }

        private List<AssemblyInformation> InitializeCoreAssemblies(string appFolder)
        {
            List<AssemblyInformation> ret = new List<AssemblyInformation>();
            foreach(var asmFile in coreAssemblies)
            {
                var asm = asmDAO.GetCoreAssembly(asmFile);
                var newAsm = new AssemblyInformation();
                if (asm != null)
                    newAsm.Code = asm.Code; // Prepare for update.

                var asmPath = Path.Combine(Environment.CurrentDirectory, asmFile);
                newAsm.Name = asmFile;
                byte[] asmBytes = File.ReadAllBytes(asmPath);
                newAsm.Version = GetFileVersion(asmBytes);
                newAsm.MD5 = MD5Sum(asmBytes);
                newAsm.Size = asmBytes.Length;
                newAsm.Date = DateTime.Now;

                if (asm == null || newAsm.Version.CompareTo(asm.Version) == 1
                    || (newAsm.Version == asm.Version && newAsm.MD5 != asm.MD5))
                {
                    asmDAO.SaveAssembly(newAsm, asmBytes);
                    ret.Add(newAsm);
                }
                else
                {
                    ret.Add(asm);
                }

            }
            return ret;
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
                    Logger.Info(String.Format("Atualizado arquivo {0} - Versao {1}", asmMeta.Name, asmMeta.Version));
                }
                else
                {
                    Logger.Warn(String.Format("Arquivo {0} - Versao {1} - não encontrado", asmMeta.Name, asmMeta.Version));
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format("Erro atualizando arquivo {0} - Versao {1}.", asmMeta.Name, asmMeta.Version), e);
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
                return sb.ToString();
            }
        }

        private bool CheckSum(string asmHash, string filename)
        {
            byte[] fileByte = File.ReadAllBytes(filename);
            return MD5Sum(fileByte) == asmHash;
        }

    }
}
