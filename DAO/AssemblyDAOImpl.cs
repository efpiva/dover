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
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Dover.Framework.DAO
{
    class AssemblyDAOImpl : AssemblyDAO
    {
        private BusinessOneDAO b1DAO;

        public AssemblyDAOImpl(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        internal override AssemblyInformation GetAssemblyInformation(string asmFile, string type)
        {
            String sql = string.Format(this.GetSQL("GetAssemblyInformation.sql"), asmFile, type);
            return b1DAO.ExecuteSqlForObject<AssemblyInformation>(sql);
        }

        internal override List<AssemblyInformation> GetAssembliesInformation(string type)
        {
            String sql = string.Format(this.GetSQL("GetAssembliesInformation.sql"), type);
            return b1DAO.ExecuteSqlForList<AssemblyInformation>(sql);
        }

        internal override byte[] GetAssembly(AssemblyInformation asm)
        {
            List<String> hexFile = b1DAO.ExecuteSqlForList<String>(
                String.Format(this.GetSQL("GetAssembly.sql"), asm.Code));
            StringBuilder sb = new StringBuilder();
            foreach (var hex in hexFile)
            {
                sb.Append(hex);
            }
            SoapHexBinary shb = SoapHexBinary.Parse(sb.ToString());
            return shb.Value;
        }

        internal override void SaveAssembly(AssemblyInformation asm, byte[] asmBytes)
        {
            string installed = (asm.Type == "C") ? "Y" : "N";
            SoapHexBinary shb = new SoapHexBinary(asmBytes);
            string asmHex = null;
            if (asmBytes != null)
                asmHex = shb.ToString();
            string sql;

            if (String.IsNullOrEmpty(asm.Code))
            {
                asm.Code = b1DAO.GetNextCode("DOVER_MODULES");
                sql = String.Format(this.GetSQL("SaveAssembly.sql"),
                        asm.Code, asm.Code, asm.Name, asm.Description, asm.FileName, asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length,
                        asm.Type, installed);
            }
            else
            {
                sql = String.Format(this.GetSQL("UpdateAssembly.sql"), asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length, asm.Code,
                    asm.Description, installed);
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteAssembly.sql"), asm.Code));
            }

            b1DAO.ExecuteStatement(sql);

            // Modules binaries
            if (asmBytes != null)
                InsertAsmBin(asm, asmHex);
        }

        private void InsertAsmBin(AssemblyInformation asm, string asmHex)
        {
            string sql;
            int maxtext = 256000;
            int insertedText = 0;

            string insertSQL = this.GetSQL("InsertAsm.sql");

            for (int i = 0; i < asmHex.Length / maxtext; i++)
            {
                string code = b1DAO.GetNextCode("DOVER_MODULES_BIN");
                sql = String.Format(insertSQL,
                    code, code, asm.Code, asmHex.Substring(i * maxtext, maxtext));
                b1DAO.ExecuteStatement(sql);
                insertedText += maxtext;
            }

            if (insertedText < asmHex.Length)
            {
                string code = b1DAO.GetNextCode("DOVER_MODULES_BIN");
                sql = String.Format(insertSQL,
                    code, code, asm.Code, asmHex.Substring(insertedText));
                b1DAO.ExecuteStatement(sql);
            }
        }

        internal override void RemoveAssembly(string moduleName)
        {
            string code = b1DAO.ExecuteSqlForObject<string>(
                string.Format(this.GetSQL("GetModuleCode.sql"), moduleName));
            if (moduleName != null)
            {
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteModule.sql"), code));
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteModuleBIN.sql"), code));
            }
        }

        internal override void SaveAssemblyI18N(string moduleCode, string i18n, byte[] i18nAsm)
        {
            string sql;
            int maxtext = 256000;
            int insertedText = 0;
            string asmHex = null;
           
            b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteModuleI18N.sql"), moduleCode));

            SoapHexBinary shb = new SoapHexBinary(i18nAsm);
            if (i18nAsm != null)
            {
                string insertSQL = this.GetSQL("InsertI18N.sql");
                asmHex = shb.ToString();
                for (int i = 0; i < asmHex.Length / maxtext; i++)
                {
                    string code = b1DAO.GetNextCode("DOVER_MODULES_I18N");
                    sql = String.Format(insertSQL,
                        code, code, moduleCode, asmHex.Substring(i * maxtext, maxtext), i18n);
                    b1DAO.ExecuteStatement(sql);
                    insertedText += maxtext;
                }

                if (insertedText < asmHex.Length)
                {
                    string code = b1DAO.GetNextCode("DOVER_MODULES_I18N");
                    sql = String.Format(insertSQL,
                        code, code, moduleCode, asmHex.Substring(insertedText), i18n);
                    b1DAO.ExecuteStatement(sql);
                }
            }
        }

        internal override bool AutoUpdateEnabled(AssemblyInformation asm)
        {
            string autoUpdateFlag = b1DAO.ExecuteSqlForObject<string>(
                string.Format(this.GetSQL("CheckAutoUpdate.sql"), asm.Code));
            return !string.IsNullOrEmpty(autoUpdateFlag) && autoUpdateFlag == "Y";
        }


        internal override List<string> GetSupportedI18N(AssemblyInformation asm)
        {
            return b1DAO.ExecuteSqlForList<string>(string.Format(this.GetSQL("GetSupportedI18N.sql"), asm.Code));
        }

        internal override byte[] GetI18NAssembly(AssemblyInformation asm, string i18n)
        {
            List<String> hexFile = b1DAO.ExecuteSqlForList<String>(
                String.Format(this.GetSQL("GetI18N.sql"), asm.Code, i18n));
            StringBuilder sb = new StringBuilder();
            foreach (var hex in hexFile)
            {
                sb.Append(hex);
            }
            SoapHexBinary shb = SoapHexBinary.Parse(sb.ToString());
            return shb.Value;
        }
    }
}
