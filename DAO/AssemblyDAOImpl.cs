using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model.Assembly;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AddOne.Framework.DAO
{
    class AssemblyDAOImpl : AssemblyDAO
    {
        private BusinessOneDAO b1DAO;

        public AssemblyDAOImpl(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        public AssemblyInformation GetCoreAssembly(string asmFile)
        {
            String sql = string.Format(@"SELECT Code, U_Name Name, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_Size Size 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'C' and U_Name = '{0}'", asmFile);
            return b1DAO.ExecuteSqlForObject<AssemblyInformation>(sql);
        }

        public List<AssemblyInformation> GetAddinsAssemblies()
        {
            String sql = @"SELECT Code, U_Name Name, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_Size Size 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'A'";
            return b1DAO.ExecuteSqlForList<AssemblyInformation>(sql);
        }

        public byte[] GetAssembly(AssemblyInformation asm)
        {
            List<String> hexFile = b1DAO.ExecuteSqlForList<String>(
                String.Format("Select U_asm from [@GA_AO_MODULES_BIN] where U_Code = '{0}'", asm.Code));
            StringBuilder sb = new StringBuilder();
            foreach (var hex in hexFile)
            {
                sb.Append(hex);
            }
            SoapHexBinary shb = SoapHexBinary.Parse(sb.ToString());
            return shb.Value;
        }

        public string GetB1StudioResource(AssemblyInformation asm)
        {
            List<String> lines = b1DAO.ExecuteSqlForList<String>(
                String.Format("Select U_Resource from [@GA_AO_MODULES_B1S] where U_Code = '{0}'", asm.Code));
            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.Append(line);
            }
            return sb.ToString();
        }

        public void SaveAssembly(AssemblyInformation asm, byte[] asmBytes)
        {
            SoapHexBinary shb = new SoapHexBinary(asmBytes);
            string asmHex = shb.ToString();
            string sql;
            int maxtext = 256000;
            int insertedText = 0;

            if (String.IsNullOrEmpty(asm.Code))
            {
                asm.Code = b1DAO.GetNextCode("GA_AO_MODULES");
                sql = String.Format(@"INSERT INTO [@GA_AO_MODULES] (Code, Name, U_Name, U_Version, U_MD5, U_Date, U_Size, U_Type)
                VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}')",
                        asm.Code, asm.Code, asm.Name, asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length, 'C');
            }
            else
            {
                sql = String.Format(@"UPDATE [@GA_AO_MODULES] Set U_Version = '{0}', U_MD5 = '{1}', U_Date = '{2}', U_Size = {3}
                             WHERE Code = '{4}'", asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length, asm.Code);
                b1DAO.ExecuteStatement(String.Format("DELETE FROM [@GA_AO_MODULES_BIN] WHERE U_Code = '{0}'", asm.Code));
            }

            b1DAO.ExecuteStatement(sql);

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
    }
}
