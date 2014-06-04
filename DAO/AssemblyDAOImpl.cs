using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AddOne.Framework.DAO
{
    public class AssemblyDAOImpl : AssemblyDAO
    {
        private BusinessOneDAO b1DAO;

        public AssemblyDAOImpl(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        public AssemblyInformation GetCoreAssembly(string asmFile)
        {
            String sql = string.Format(@"SELECT Code, U_Name Name, ISNULL(U_Description, U_Name) Description, U_FileName FileName, U_ResourceName ResourceName, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_Size Size, U_Type Type 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'C' and U_Name = '{0}'", asmFile);
            return b1DAO.ExecuteSqlForObject<AssemblyInformation>(sql);
        }

        public AssemblyInformation GetAddInAssembly(string asmFile)
        {
            String sql = string.Format(@"SELECT Code, U_Name Name, ISNULL(U_Description, U_Name) Description, U_FileName FileName, U_ResourceName ResourceName, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_Size Size, U_Type Type 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'A' and U_Name = '{0}'", asmFile);
            return b1DAO.ExecuteSqlForObject<AssemblyInformation>(sql);
        }

        public List<AssemblyInformation> GetAddinsAssemblies()
        {
            String sql = @"SELECT Code, U_Name Name, ISNULL(U_Description, U_Name) Description, U_FileName FileName, U_ResourceName ResourceName, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_Size Size, U_Type Type 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'A'";
            return b1DAO.ExecuteSqlForList<AssemblyInformation>(sql);
        }

        public List<AssemblyInformation> GetCoreAssemblies()
        {
            String sql = @"SELECT Code, U_Name Name, ISNULL(U_Description, U_Name) Description, U_FileName FileName, U_ResourceName ResourceName, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_Size Size, U_Type Type 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'C'";
            return b1DAO.ExecuteSqlForList<AssemblyInformation>(sql);
        }
        
        public byte[] GetAssembly(AssemblyInformation asm)
        {
            List<String> hexFile = b1DAO.ExecuteSqlForList<String>(
                String.Format("Select U_asm from [@GA_AO_MODULES_BIN] where U_Code = '{0}' ORDER BY Code", asm.Code));
            StringBuilder sb = new StringBuilder();
            foreach (var hex in hexFile)
            {
                sb.Append(hex);
            }
            SoapHexBinary shb = SoapHexBinary.Parse(sb.ToString());
            return shb.Value;
        }

        public byte[] GetB1StudioResource(AssemblyInformation asm)
        {
            List<String> lines = b1DAO.ExecuteSqlForList<String>(
                String.Format("Select U_Resource from [@GA_AO_MODULES_B1S] where U_Code = '{0}' ORDER BY Code", asm.Code));
            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.Append(line);
            }
            SoapHexBinary shb = SoapHexBinary.Parse(sb.ToString());
            return shb.Value;
        }

        public void SaveAssembly(AssemblyInformation asm, byte[] asmBytes, byte[] b1sbytes)
        {
            SoapHexBinary shb = new SoapHexBinary(asmBytes);
            SoapHexBinary shbb1s = new SoapHexBinary(b1sbytes);
            string asmHex = null, b1SResource = null;
            if (asmBytes != null)
                asmHex = shb.ToString();
            if (b1sbytes != null)
                b1SResource = shbb1s.ToString();
            string sql;

            if (String.IsNullOrEmpty(asm.Code))
            {
                asm.Code = b1DAO.GetNextCode("GA_AO_MODULES");
                sql = String.Format(@"INSERT INTO [@GA_AO_MODULES] (Code, Name, U_Name, U_Description, U_FileName, U_ResourceName, U_Version, U_MD5, U_Date, U_Size, U_Type, U_Status)
                VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, '{10}', 'A')",
                        asm.Code, asm.Code, asm.Name, asm.Description, asm.FileName, asm.ResourceName, asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length, asm.Type);
            }
            else
            {
                sql = String.Format(@"UPDATE [@GA_AO_MODULES] Set U_Version = '{0}', U_MD5 = '{1}', U_Date = '{2}', U_Size = {3}, U_Description = '{5}'
                             WHERE Code = '{4}'", asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length, asm.Code, asm.Description);
                b1DAO.ExecuteStatement(String.Format("DELETE FROM [@GA_AO_MODULES_BIN] WHERE U_Code = '{0}'", asm.Code));
                b1DAO.ExecuteStatement(String.Format("DELETE FROM [@GA_AO_MODULES_B1S] WHERE U_Code = '{0}'", asm.Code));
            }

            b1DAO.ExecuteStatement(sql);

            // Modules binaries
            if (asmBytes != null)
                InsertAsmBin(asm, asmHex);

            // B1Studio resource
            if (b1SResource != null)
                InsertB1SResource(asm, b1SResource);
        }

        private void InsertAsmBin(AssemblyInformation asm, string asmHex)
        {
            string sql;
            int maxtext = 256000;
            int insertedText = 0;

            for (int i = 0; i < asmHex.Length / maxtext; i++)
            {
                string code = b1DAO.GetNextCode("GA_AO_MODULES_BIN");
                sql = String.Format("INSERT INTO [@GA_AO_MODULES_BIN] (Code, Name, U_Code, U_Asm) VALUES ('{0}', '{1}', '{2}', '{3}')",
                    code, code, asm.Code, asmHex.Substring(i * maxtext, maxtext));
                b1DAO.ExecuteStatement(sql);
                insertedText += maxtext;
            }

            if (insertedText < asmHex.Length)
            {
                string code = b1DAO.GetNextCode("GA_AO_MODULES_BIN");
                sql = String.Format("INSERT INTO [@GA_AO_MODULES_BIN] (Code, Name, U_Code, U_Asm) VALUES ('{0}', '{1}', '{2}', '{3}')",
                    code, code, asm.Code, asmHex.Substring(insertedText));
                b1DAO.ExecuteStatement(sql);
            }
        }

        private void InsertB1SResource(AssemblyInformation asm, string b1SResource)
        {
            string sql;
            int maxtext = 256000;
            int insertedText = 0;

            for (int i = 0; i < b1SResource.Length / maxtext; i++)
            {
                string code = b1DAO.GetNextCode("GA_AO_MODULES_B1S");
                sql = String.Format("INSERT INTO [@GA_AO_MODULES_B1S] (Code, Name, U_Code, U_Resource) VALUES ('{0}', '{1}', '{2}', '{3}')",
                    code, code, asm.Code, b1SResource.Substring(i * maxtext, maxtext));
                b1DAO.ExecuteStatement(sql);
                insertedText += maxtext;
            }

            if (insertedText < b1SResource.Length)
            {
                string code = b1DAO.GetNextCode("GA_AO_MODULES_B1S");
                sql = String.Format("INSERT INTO [@GA_AO_MODULES_B1S] (Code, Name, U_Code, U_Resource) VALUES ('{0}', '{1}', '{2}', '{3}')",
                    code, code, asm.Code, b1SResource.Substring(insertedText));
                b1DAO.ExecuteStatement(sql);
            }
        }

        public void RemoveAsm(string moduleName)
        {
            string code = b1DAO.ExecuteSqlForObject<string>(
                string.Format("SELECT Code FROM [@GA_AO_MODULES] WHERE U_Name = '{0}'", moduleName));
            if (moduleName != null)
            {
                b1DAO.ExecuteStatement(String.Format("DELETE FROM [@GA_AO_MODULES] WHERE Code = '{0}'", code));
                b1DAO.ExecuteStatement(String.Format("DELETE FROM [@GA_AO_MODULES_B1S] WHERE U_Code = '{0}'", code));
                b1DAO.ExecuteStatement(String.Format("DELETE FROM [@GA_AO_MODULES_BIN] WHERE U_Code = '{0}'", code));
            }
        }
    }
}
