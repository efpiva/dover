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
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using Dover.Framework.Model;

namespace Dover.Framework.DAO
{
    class AssemblyDAOImpl : AssemblyDAO
    {
        private BusinessOneDAO b1DAO;

        public AssemblyDAOImpl(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        internal override AssemblyInformation GetAssemblyInformation(string asmFile, AssemblyType type)
        {
            String sql = string.Format(this.GetSQL("GetAssemblyInformation.sql"), asmFile, AssemblyInformation.ConvertTypeToCode(type));
            return b1DAO.ExecuteSqlForObject<AssemblyInformation>(sql);
        }

        internal override List<AssemblyInformation> GetAssembliesInformation(AssemblyType type)
        {
            String sql = string.Format(this.GetSQL("GetAssembliesInformation.sql"), AssemblyInformation.ConvertTypeToCode(type));
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
            return Compression.Uncompress(shb.Value);
        }


        internal override void SaveAssemblyDependency(AssemblyInformation newAsm,
               AssemblyInformation dependency, byte[] dependencyBytes)
        {
            dependency.Code = null; // force insert.
            SaveAssembly(dependency, dependencyBytes);
            SaveAssemblyDependency(newAsm, dependency.Code);
        }

        internal override void SaveAssembly(AssemblyInformation asm, byte[] asmBytes)
        {
            string installed = (asm.Type ==AssemblyType.Core) ? "Y" : "N";
            SoapHexBinary shb = new SoapHexBinary(Compression.Compress(asmBytes));
            string asmHex = null;
            if (asmBytes != null)
                asmHex = shb.ToString();
            string sql;

            if (String.IsNullOrEmpty(asm.Code))
            {
                asm.Code = b1DAO.GetNextCode("DOVER_MODULES");
                sql = String.Format(this.GetSQL("SaveAssembly.sql"),
                        asm.Code, asm.Code, asm.Name, asm.Description, asm.FileName, asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length,
                        asm.TypeCode, installed);
            }
            else
            {
                sql = String.Format(this.GetSQL("UpdateAssembly.sql"), asm.Version, asm.MD5, asm.Date.ToString("yyyyMMdd"), asmBytes.Length, asm.Code,
                    asm.Description, installed);
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteAssembly.sql"), asm.Code));
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteDependencies.sql"), asm.Code));
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

        internal override void RemoveAssembly(string code)
        {
            if (code != null)
            {
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteModule.sql"), code));
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteAssembly.sql"), code));
                b1DAO.ExecuteStatement(String.Format(this.GetSQL("DeleteDependencies.sql"), code));
            }
        }

        internal override bool AutoUpdateEnabled(AssemblyInformation asm)
        {
            string autoUpdateFlag = b1DAO.ExecuteSqlForObject<string>(
                string.Format(this.GetSQL("CheckAutoUpdate.sql"), asm.Code));
            return !string.IsNullOrEmpty(autoUpdateFlag) && autoUpdateFlag == "Y";
        }

        internal override List<AssemblyInformation> GetAutoUpdateAssemblies()
        {
            return b1DAO.ExecuteSqlForList<AssemblyInformation>(this.GetSQL("GetAutoUpdateAssembliesInformation.sql"));
        }

        internal override List<AssemblyInformation> GetDependencies(AssemblyInformation asm)
        {
            return b1DAO.ExecuteSqlForList<AssemblyInformation>(string.Format(this.GetSQL("GetDependencyInformation.sql"), asm.Code));
        }

        internal override int GetDependencyCount(AssemblyInformation dep)
        {
            return b1DAO.ExecuteSqlForObject<int>(string.Format(this.GetSQL("GetDependencyCount.sql"), dep.Code));
        }

        internal override string GetDependencyCode(string md5)
        {
            return b1DAO.ExecuteSqlForObject<string>(string.Format(this.GetSQL("GetDependencyByMD5.sql"), md5));
        }

        internal override void SaveAssemblyDependency(AssemblyInformation newAsm, string dependencyCode)
        {
            string newCode = b1DAO.GetNextCode("DOVER_MODULES_DEP");
            b1DAO.ExecuteStatement(string.Format(this.GetSQL("InsertDependency.sql"), newCode, newAsm.Code, dependencyCode));
        }

        internal override void DeleteOrphanDependency()
        {
            List<string> orphanCodes = b1DAO.ExecuteSqlForList<string>(this.GetSQL("GetOrphanCodes.sql"));
            foreach (var code in orphanCodes)
            {
                RemoveAssembly(code);
            }
        }
    }
}
