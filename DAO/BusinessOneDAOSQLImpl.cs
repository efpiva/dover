using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Castle.Core.Logging;
using System.Reflection;
using AddOne.Framework.Model.SAP;
using AddOne.Framework.Model.SAP.Assembly;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace AddOne.Framework.DAO
{

    public class MyXmlTextWriter : XmlTextWriter
    {
        public MyXmlTextWriter(Stream stream)
            : base(stream, Encoding.Unicode)
        {

        }

        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }
    }

    class BusinessOneDAOSQLImpl : BusinessOneDAO
    {
        private Company company;
        private SAPbouiCOM.Application application;

        public ILogger Logger { get; set; }

        public BusinessOneDAOSQLImpl(Company company, SAPbouiCOM.Application application)
        {
            this.company = company;
            this.application = application;
        }


        public void CreateTable(GAUserTable table)
        {
            var tables = new List<GAUserTable>();
            tables.Add(table);
            CreateTables(tables);
        }

        public void CreateField(GAUserField field)
        {
            var fields = new List<GAUserField>();
            fields.Add(field);
            CreateFields(fields);
        }

        public void CreateTables(List<GAUserTable> tables)
        {
            if (tables == null || tables.Count == 0)
                return;
            
            var bom = new GAUserTableBOM();
            bom.BO = new List<GAUserTableMD>();

            foreach (var table in tables)
            {
                var utmd = new GAUserTableMD();
                bom.BO.Add(utmd);
                utmd.AdmInfo = new AddOne.Framework.Model.SAP.AdminInfo();
                utmd.AdmInfo.Object = "153";
                utmd.AdmInfo.Version = 2;
                utmd.UserTableMD.Add(table);
            }

            var xnameSpace = new XmlSerializerNamespaces();
            XmlAttributeOverrides xOver = new XmlAttributeOverrides();
            XmlAttributes attrs = new XmlAttributes();
            attrs.XmlIgnore = true;
            xOver.Add(typeof(GAUserTable), "UserFields", attrs);
            xnameSpace.Add("", "");
            var serializer = new XmlSerializer(typeof(GAUserTableBOM), xOver);
            var writer = new StringWriter();

            serializer.Serialize(writer, bom, xnameSpace);
            CreateTablesFromXML(writer.ToString());
        }

        public void CreateFields(List<GAUserField> fields)
        {
            if (fields == null || fields.Count == 0)
                return;

            var bom = new GAUserFieldBOM();
            bom.BO = new List<GAUserFieldMD>();

            foreach (var field in fields)
            {
                var ufmd = new GAUserFieldMD();
                bom.BO.Add(ufmd);
                ufmd.AdmInfo = new AddOne.Framework.Model.SAP.AdminInfo();
                ufmd.AdmInfo.Object = "152";
                ufmd.AdmInfo.Version = 2;
                ufmd.UserFieldMD.Add(field);
                ufmd.ValidValuesMD = field.ValidValuesMD;
            }

            var xnameSpace = new XmlSerializerNamespaces();
            xnameSpace.Add("", "");
            var serializer = new XmlSerializer(typeof(GAUserFieldBOM));

            var stream = new MemoryStream();
            var xmlWriter = new MyXmlTextWriter(stream);

            serializer.Serialize(xmlWriter, bom, xnameSpace);
            CreateFieldsFromXML(Encoding.Unicode.GetString(stream.ToArray()));
        }

        public List<GAUserTable> CheckTablesExists(List<GAUserTable> tables)
        {
            int searchHead = 0;
            var ret = new List<GAUserTable>();

            if (tables == null || tables.Count == 0)
                return ret;

            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(CheckTablesSQL(tables));
            tables.Sort();

            /**
             * Using searchHead all search operation is done in O(n) of tables, at once.
             */
            string tableName = "";
            while (!rs.EoF)
            {
                tableName = (string)rs.Fields.Item("TableName").Value;
                for (; searchHead < tables.Count && tableName != tables[searchHead].TableName; searchHead++)
                {
                    ret.Add(tables[searchHead]);
                }
                if (tableName == tables[searchHead].TableName)
                    searchHead++;
                rs.MoveNext();
            }

            for (; searchHead < tables.Count && tableName != tables[searchHead].TableName; searchHead++)
            {
                ret.Add(tables[searchHead]);
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            return ret;
        }

        private string CheckTablesSQL(List<GAUserTable> tables)
        {
            StringBuilder sqlSb = new StringBuilder("select TableName from OUTB where OUTB.TableName in (");
            bool first = true;
            foreach (var table in tables)
            {
                if (!first)
                    sqlSb.Append(", ");
                sqlSb.Append(String.Format("'{0}'", table.TableName));
                first = false;
            }
            sqlSb.Append(") order by TableName");
            return sqlSb.ToString();
        }

        public List<GAUserField> CheckFieldsExists(List<GAUserTable> tables)
        {
            var outRet = new List<GAUserField>();
            foreach (var table in tables)
            {
                var fields = table.UserFields;
                int searchHead = 0;
                var ret = new List<GAUserField>();

                if (fields == null || fields.Count == 0)
                    return ret;

                Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery(CheckFieldsSQL(fields));
                fields.Sort();

                /**
                 * Using searchHead all search operation is done in O(n) of tables, at once.
                 */
                string fieldName = "";
                string tableName = "";
                while (!rs.EoF)
                {
                    fieldName = (string)rs.Fields.Item("AliasID").Value;
                    tableName = (string)rs.Fields.Item("TableID").Value;
                    for (; searchHead < fields.Count && (fieldName != fields[searchHead].Name
                                || tableName != fields[searchHead].TableName); searchHead++)
                    {
                        ret.Add(fields[searchHead]);
                    }
                    if (tableName == fields[searchHead].TableName && fieldName == fields[searchHead].Name)
                        searchHead++;

                    rs.MoveNext();
                }
                for (; searchHead < fields.Count && (fieldName != fields[searchHead].Name
                                || tableName != fields[searchHead].TableName); searchHead++)
                {
                    ret.Add(fields[searchHead]);
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                outRet.AddRange(ret);
            }
            return outRet;
        }

        private string CheckFieldsSQL(List<GAUserField> fields)
        {
            StringBuilder sqlSb = new StringBuilder("select AliasID, TableID from CUFD where ");
            bool first = true;
            foreach (var field in fields)
            {
                if (!first)
                    sqlSb.Append(" or ");
                sqlSb.Append(String.Format("(TableID = '{0}' and AliasID = '{1}')", field.TableName, field.Name));
                first = false;
            }
            sqlSb.Append(" order by TableID, AliasID");
            return sqlSb.ToString();
        }

        private void CreateFieldsFromXML(string xmlFields)
        {
            int result;
            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            for (int i = 0; i < company.GetXMLelementCount(xmlFields); i++)
            {
                var objUserField = (SAPbobsCOM.UserFieldsMD)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                objUserField.Browser.ReadXml(xmlFields, i);
                Logger.Info(String.Format("Criando campo [{0}].{1}", objUserField.TableName, objUserField.Name));
                result = objUserField.Add();
                if ((result != 0) && (result != -2035))
                {
                    string errMsg = company.GetLastErrorDescription();
                    Logger.Error(String.Format("Erro ao criar campo de usuário[{0}].{1} {2}", objUserField.TableName,
                        objUserField.Name, errMsg));
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserField);
            }

        }

        private void CreateTablesFromXML(string xmlTables)
        {
            int result;
            var objUserTable = (SAPbobsCOM.UserTablesMD)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            for (int i = 0; i < company.GetXMLelementCount(xmlTables); i++)
            {
                objUserTable.Browser.ReadXml(xmlTables, i);
                Logger.Info(String.Format("Criando a tabela [{0}]", objUserTable.TableName));
                result = objUserTable.Add();
                if ((result != 0) && (result != -2035))
                {
                    string errMsg = company.GetLastErrorDescription();
                    Logger.Error(String.Format("Erro ao criar Tabela de Usuário [{0}] {1}", objUserTable.TableName, errMsg));
                }
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserTable);
        }


        public string GetNextCode(string udt)
        {
            SAPbobsCOM.Recordset objRecordSet = (SAPbobsCOM.Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string id;

            String query = String.Format(" Select Max(Code) From [@{0}] ", udt);
            objRecordSet.DoQuery(query);

            if (objRecordSet.Fields.Item(0).Value.ToString() == "")
            {
                id = "00000001";
            }
            else
            {
                bool dbo_IncreaseCode;
                int din_MyValue;
                string dst_MyNewCode, pst_Code;
                char dst_Caracter;

                pst_Code = objRecordSet.Fields.Item(0).Value.ToString();

                dst_MyNewCode = "";
                dbo_IncreaseCode = true;
                for (int i = 0; i <= 7; i++)
                {
                    if (dbo_IncreaseCode == true)
                    {
                        dst_Caracter = pst_Code[7 - i];
                        din_MyValue = Convert.ToInt32(dst_Caracter);
                        if ((din_MyValue >= 48) && (din_MyValue < 57))  //the value ascii code is between 0 and 9, so it should be incresed by 1
                        {
                            dbo_IncreaseCode = false;
                            dst_MyNewCode = (char)(din_MyValue + 1) + dst_MyNewCode;
                        }
                        else if (din_MyValue == 57) //the value is 9, the next one would be A
                        {
                            dbo_IncreaseCode = false;
                            dst_MyNewCode = (char)(65) + dst_MyNewCode;
                        }
                        else if ((din_MyValue >= 65) && (din_MyValue < 90))  //The ascii code is between A and Z
                        {
                            dbo_IncreaseCode = false;
                            dst_MyNewCode = (char)(din_MyValue + 1) + dst_MyNewCode;
                        }
                        else if (din_MyValue == 90)     // the value is Z, It has to come back to 1, and inceease the next digit
                        {
                            dbo_IncreaseCode = true;    // this allow to increse the next digit
                            dst_MyNewCode = (char)(48) + dst_MyNewCode;
                        }
                    }
                    else
                    {
                        dst_MyNewCode = pst_Code.Substring(7 - i, 1) + dst_MyNewCode;
                    }
                }

                id = dst_MyNewCode;
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(objRecordSet);
            return (id);
        }

        public string GetCurrentUser()
        {
            return company.UserName;
        }

        public void ExecuteStatement(string sql)
        {
            var objRecordSet = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                objRecordSet.DoQuery(sql);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objRecordSet);
            }
        }

        public List<AssemblyInformation> GetCoreAssemblies()
        {
            String sql = @"SELECT U_Name Name, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_History History, U_Size Size 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'C'";
            return ExecuteSqlForList<AssemblyInformation>(sql);
        }

        public List<AssemblyInformation> GetAddinsAssemblies()
        {
            String sql = @"SELECT U_Name Name, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_History History, U_Size Size 
                            FROM [@GA_AO_MODULES]
                                where U_Type = 'A'";
            return ExecuteSqlForList<AssemblyInformation>(sql);
        }

        public byte[] GetAssembly(AssemblyInformation asm)
        {
            String hexFile = ExecuteSqlForObject<String>(
                String.Format("Select U_asm from [@GA_AO_MODULES] where Code = '{0}'", asm.Code));
            SoapHexBinary shb = SoapHexBinary.Parse(hexFile);
            return shb.Value;
        }

        public T ExecuteSqlForObject<T>(string sql)
        {
            Type type = typeof(T);
            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                rs.DoQuery(sql);
                int fieldsSize = rs.Fields.Count;
                if (!rs.EoF)
                {
                    if (IsNotCoreType(type))
                    {
                        object obj = rs.Fields.Item(0);
                        if (obj.GetType() != type)
                        {
                            String errMsg = String.Format(
                                "Objeto do tipo {0}. Retorno da consulta no tipo {1}.",
                                obj.GetType(), type);
                            Logger.Error(errMsg);
                            throw new Exception(errMsg);
                        }
                        return (T)obj;
                    }
                    else
                    {
                        return PrepareObject<T>(rs);
                    }
                }
                return default(T);
            }
            catch (Exception e)
            {
                Logger.Error(String.Format("Erro executando SQL: {0}", e.Message), e);
                throw e;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }
        }

        private bool IsNotCoreType(Type type)
        {
            return (type != typeof(object) && Type.GetTypeCode(type) == TypeCode.Object);
        }

        public List<T> ExecuteSqlForList<T>(string sql)
        {
            var retValue = new List<T>();
            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                rs.DoQuery(sql);
                int fieldsSize = rs.Fields.Count;
                while (!rs.EoF)
                {
                    T obj = PrepareObject<T>(rs);
                    retValue.Add(obj);
                    rs.MoveNext();
                }
                return retValue;
            }
            catch (Exception e)
            {
                Logger.Error(String.Format("Erro executando SQL: {0}", e.Message), e);
                throw e;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }
        }

        private T PrepareObject<T>(Recordset rs)
        {
            Type type = typeof(T);
            T obj = (T)Activator.CreateInstance(type);
            for (int i = 0; i < rs.Fields.Count; i++)
            {
                string name = rs.Fields.Item(i).Name;
                try
                {
                    object value = rs.Fields.Item(i).Name;
                    var prop = type.GetProperty(name);
                    if (prop == null)
                    {
                        String errMsg = String.Format("Objeto {0} não tem tem propriedade {1}", type, name);
                        Logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }
                    if (prop.PropertyType != value.GetType())
                    {
                        String errMsg = String.Format(
                            "Objeto {0} tem propriedade {1} do tipo {2}. Retorno da consulta no tipo {3}.",
                            type, name, prop.PropertyType, value.GetType());
                        Logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }
                    prop.SetValue(obj, value, null);
                }
                catch (Exception e)
                {
                    Logger.Error(String.Format("Erro setando propriedade {0}", name), e);
                    throw e;
                }
            }
            return obj;
        }
    }
}
