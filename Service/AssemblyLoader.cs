using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;
using AddOne.Framework.Model.SAP.Assembly;
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
        private const string[] coreAssemblies = new string[] {
            "Framework.dll",
            "lo4j.dll",
            "Castle.Core.dll",
            "Castle.Facilities.Logging.dll",
            "Castle.Windsor.dll",
            "AddOne.exe"
        };
        private BusinessOneDAO b1DAO;
        public ILogger Logger { get; set; }


        public AssemblyLoader(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        internal void UpdateAssemblies(AssemblySource assemblyLocation, string appFolder)
        {
            List<AssemblyInformation> asms;
            if (assemblyLocation == AssemblySource.Core)
            {
                asms = b1DAO.GetCoreAssemblies();
                if (asms == null || asms.Count == 0)
                {
                    asms = InitializeCoreAssemblies(appFolder);
                }
            }
            else
                asms = b1DAO.GetAddinsAssemblies();

            foreach(var asm in asms)
            {
                string fullPath = Path.Combine(appFolder, asm.FileName);
                if (!IsDifferent(asm, appFolder, fullPath))
                {
                    UpdateAssembly(asm, fullPath);
                }
            }
        }

        private List<AssemblyInformation> InitializeCoreAssemblies(string appFolder)
        {
            List<AssemblyInformation> ret = new List<AssemblyInformation>();
            foreach(var asmFile in coreAssemblies)
            {
                var asm = new AssemblyInformation();
                var asmPath = Path.Combine(Environment.CurrentDirectory, asmFile);
                asm.FileName = asmFile;
                byte[] asmBytes = File.ReadAllBytes(asmPath);
                asm.Version = GetFileVersion(asmBytes);
                asm.MD5 = MD5Sum(asmPath);
                SaveAssembly(asm, asmBytes);
                ret.Add(asm);
            }
            return ret;
        }

        private void SaveAssembly(AssemblyInformation asm, byte[] asmBytes)
        {
            SoapHexBinary shb = new SoapHexBinary(asmBytes);
            string asmHex = shb.ToString();

            asm.Code = b1DAO.GetNextCode("GA_AO_MODULES");
            string sql = String.Format(@"INSERT INTO [@GA_AO_MODULES] (Code, Name, U_Name, U_Version, U_MD5, U_Date, U_Size, U_Type, U_Asm)
                Values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, '{8}', '{9}')",
                    asm.Code, asm.Code, asm.FileName, asm.Version, asm.MD5, DateTime.Now.ToString("yyyyMMdd"), asmBytes.Length, 'C', asmHex);

            b1DAO.ExecuteStatement(sql);
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
                byte[] asm = b1DAO.GetAssembly(asmMeta);
                if (asm != null)
                {
                    File.WriteAllBytes(fullPath, asm);
                    Logger.Info(String.Format("Atualizado arquivo {0} - Versao {1}", asmMeta.FileName, asmMeta.Version));
                }
                else
                {
                    Logger.Warn(String.Format("Arquivo {0} - Versao {1} - não encontrado", asmMeta.FileName, asmMeta.Version));
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format("Erro atualizando arquivo {0} - Versao {1}.", asmMeta.FileName, asmMeta.Version), e);
            }
        }

        private bool IsDifferent(AssemblyInformation asm, string appFolder, string fullPath)
        {
            return !File.Exists(fullPath) || !CheckSum(asm.MD5, fullPath);
        }

        private string MD5Sum(string filename)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filename))
            {
                byte[] hash = md5.ComputeHash(stream);
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
            return MD5Sum(filename) == asmHash;
        }

    }
}
