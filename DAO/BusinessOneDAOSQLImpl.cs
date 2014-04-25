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
using AddOne.Framework.Monad;
using AddOne.Framework.Model.SAP;
using AddOne.Framework.Model.Assembly;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Xml.Linq;

namespace AddOne.Framework.DAO
{

    class BusinessOneDAOSQLImpl : BusinessOneDAO
    {
        private Company company;

        public ILogger Logger { get; set; }

        public BusinessOneDAOSQLImpl(Company company)
        {
            this.company = company;
        }


        public void SaveBOMIfNotExists(UserTableBOM bom)
        {
            bom = bom.Do(x => FilterMissingTablesFromBOM(bom));

            var xml = this.With(x => bom).Serialize<UserTableBOM>();
    
            if (xml != null)
                CreateTablesFromXML(xml);
        }

        public void SaveBOMIfNotExists(UserFieldBOM bom)
        {
            if (bom == null || bom.BO == null || bom.BO.Length == 0)
                return;

            bom = FilterMissingFieldsFromBOM(bom);

            var xml = bom.Serialize<UserFieldBOM>();
            CreateFieldsFromXML(xml);
        }

        public void UpdateOrSaveBOMIfNotExists(FormattedSearchBOM bom)
        {
            FormattedSearches fs = null;
            if (bom == null || bom.BO == null || bom.BO.Length == 0)
                return;
            String xmlBom = bom.Serialize();

            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            for (int i = 0; i < company.GetXMLelementCount(xmlBom); i++)
            {
                try
                {
                    fs = (FormattedSearches)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oFormattedSearches);
                    fs.Browser.ReadXml(xmlBom, i);
                    var resourceFS = XDocument.Parse(fs.GetAsXML());
                    var index = fs.Index;
                    if (fs.GetByKey(index))
                    {
                        UpdateFS(fs, resourceFS);
                    }
                    else
                    {
                        AddFS(fs, xmlBom, i);
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(fs);
                }
            }
        }

        private void AddFS(FormattedSearches fs, string xmlBom, int index)
        {
            fs.Browser.ReadXml(xmlBom, index);
            int ret = fs.Add();
            if (ret != 0)
            {
                string err;
                company.GetLastError(out ret, out err);
                string exceptionErr = String.Format("Erro criando Pesquisa Formatada {0} - {1}", fs.Index, err);
                Logger.Error(exceptionErr);
            }
        }

        private void UpdateFS(FormattedSearches fs, XDocument resourceFS)
        {
            var currFS = XDocument.Parse(fs.GetAsXML());
            if (!XDocument.DeepEquals(currFS, resourceFS))
            {
                fs.Browser.ReadXml(resourceFS.ToString(), 0);
                int ret = fs.Update();
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    string exceptionErr = String.Format("Erro atualizando Pesquisa Formatada {0} - {1}", fs.Index, err);
                    Logger.Error(exceptionErr);
                }
            }
        }

        public void UpdateOrSaveBOMIfNotExists(UserQueriesBOM bom)
        {
            UserQueries uq = null;
            if (bom == null || bom.BO == null || bom.BO.Length == 0)
                return;
            String xmlBom = bom.Serialize();

            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            for (int i = 0; i < company.GetXMLelementCount(xmlBom); i++)
            {
                try
                {
                    uq = (UserQueries)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserQueries);
                    uq.Browser.ReadXml(xmlBom, i);
                    var resourceFS = XDocument.Parse(uq.GetAsXML());
                    var internalKey = uq.InternalKey;
                    var category = uq.QueryCategory;
                    if (uq.GetByKey(internalKey, category))
                    {
                        UpdateUQ(uq, resourceFS);
                    }
                    else
                    {
                        AddUQ(uq, xmlBom, i);
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(uq);
                }
            }
        }

        private void AddUQ(UserQueries uq, string xmlBom, int index)
        {
            uq.Browser.ReadXml(xmlBom, index);
            int ret = uq.Add();
            if (ret != 0)
            {
                string err;
                company.GetLastError(out ret, out err);
                string exceptionErr = String.Format("Erro criando Consulta de Usuário {0}:{1} - {2}",
                    uq.InternalKey, uq.QueryCategory, err);
                Logger.Error(exceptionErr);
            }
        }

        private void UpdateUQ(UserQueries uq, XDocument resourceFS)
        {
            var currFS = XDocument.Parse(uq.GetAsXML());
            if (!XDocument.DeepEquals(currFS, resourceFS))
            {
                uq.Browser.ReadXml(resourceFS.ToString(), 0);
                int ret = uq.Update();
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    string exceptionErr = String.Format("Erro atualizando Consulta de Usuário {0}:{1} - {2}", 
                        uq.InternalKey, uq.QueryCategory, err);
                    Logger.Error(exceptionErr);
                }
            }
        }

        public void UpdateOrSaveBOMIfNotExists(QueryCategoriesBOM bom)
        {
            QueryCategories qc = null;
            if (bom == null || bom.BO == null || bom.BO.Length == 0)
                return;
            String xmlBom = bom.Serialize();

            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            for (int i = 0; i < company.GetXMLelementCount(xmlBom); i++)
            {
                try
                {
                    qc = (QueryCategories)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oQueryCategories);
                    qc.Browser.ReadXml(xmlBom, i);
                    var resourceFS = XDocument.Parse(qc.GetAsXML());
                    var id = qc.Code;
                    if (qc.GetByKey(id))
                    {
                        UpdateQC(qc, resourceFS);
                    }
                    else
                    {
                        AddQC(qc, xmlBom, i);
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(qc);
                }
            }
        }

        private void AddQC(QueryCategories qc, string xmlBom, int index)
        {
            qc.Browser.ReadXml(xmlBom, index);
            int ret = qc.Add();
            if (ret != 0)
            {
                string err;
                company.GetLastError(out ret, out err);
                string exceptionErr = String.Format("Erro criando Consulta de Usuário {0} - {1}",
                    qc.Code, err);
                Logger.Error(exceptionErr);
            }
        }

        private void UpdateQC(QueryCategories qc, XDocument resourceFS)
        {
            var currFS = XDocument.Parse(qc.GetAsXML());
            if (!XDocument.DeepEquals(currFS, resourceFS))
            {
                qc.Browser.ReadXml(resourceFS.ToString(), 0);
                int ret = qc.Update();
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    string exceptionErr = String.Format("Erro atualizando Consulta de Usuário {0} - {1}",
                        qc.Code, err);
                    Logger.Error(exceptionErr);
                }
            }
        }

        public void UpdateOrSaveBOMIfNotExists(UDOBOM bom)
        {
            UserObjectsMD udo = null;
            if (bom == null || bom.BO == null || bom.BO.Length == 0)
                return;
            String xmlBom = bom.Serialize<UDOBOM>();

            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            for (int i = 0; i < company.GetXMLelementCount(xmlBom); i++)
            {
                try
                {
                    udo = (UserObjectsMD)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                    udo.Browser.ReadXml(xmlBom, i);
                    var resourceUDO = XDocument.Parse(udo.GetAsXML());
                    var code = udo.Code;
                    if (udo.GetByKey(code))
                    {
                        UpdateUDO(udo, resourceUDO);
                    }
                    else
                    {
                        AddUDO(udo, xmlBom, i);
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(udo);
                }
            }
        }

        private void AddUDO(UserObjectsMD udo, String xmlBom, int index)
        {
            udo.Browser.ReadXml(xmlBom, index);
            int ret = udo.Add();
            if (ret != 0)
            {
                string err;
                company.GetLastError(out ret, out err);
                string exceptionErr = String.Format("Erro criando UDO {0} - {1}", udo.Code, err);
                Logger.Error(exceptionErr);
            }
        }

        private void UpdateUDO(UserObjectsMD udo, XDocument resourceUDO)
        {
            var currUdo = XDocument.Parse(udo.GetAsXML());
            if (!XDocument.DeepEquals(currUdo, resourceUDO))
            {
                udo.Browser.ReadXml(resourceUDO.ToString(), 0);
                int ret = udo.Update();
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    string exceptionErr = String.Format("Erro atualizando UDO {0} - {1}", udo.Code, err);
                    Logger.Error(exceptionErr);
                }
            }
        }

        public void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute)
        {
            UserPermissionTree userPermissionTree = null;

            try
            {
                userPermissionTree = (UserPermissionTree)company.GetBusinessObject(BoObjectTypes.oUserPermissionTree);
                if (userPermissionTree.GetByKey(permissionAttribute.PermissionID))
                {
                    UpdatePermission(userPermissionTree, permissionAttribute);
                }
                else
                {
                    AddPermission(userPermissionTree, permissionAttribute);
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(userPermissionTree);
            }
        }

        private void AddPermission(UserPermissionTree userPermissionTree, Attribute.PermissionAttribute permissionAttribute)
        {
            userPermissionTree.PermissionID = permissionAttribute.PermissionID;
            userPermissionTree.Name = permissionAttribute.Name;
            userPermissionTree.ParentID = permissionAttribute.ParentID;
            userPermissionTree.UserPermissionForms.FormType = permissionAttribute.FormType;
            userPermissionTree.Options = permissionAttribute.Options;

            int ret = userPermissionTree.Add();
            if (ret != 0)
            {
                string err;
                company.GetLastError(out ret, out err);
                string exceptionErr = String.Format("Erro adicionando Permissão {0} - {1}", permissionAttribute.PermissionID, err);
                Logger.Error(exceptionErr);
            }

        }

        private void UpdatePermission(UserPermissionTree userPermissionTree, Attribute.PermissionAttribute permissionAttribute)
        {
            var currPerm = XDocument.Parse(userPermissionTree.GetAsXML());
            userPermissionTree.PermissionID = permissionAttribute.PermissionID;
            userPermissionTree.Name = permissionAttribute.Name;
            userPermissionTree.ParentID = permissionAttribute.ParentID;
            userPermissionTree.UserPermissionForms.FormType = permissionAttribute.FormType;
            userPermissionTree.Options = permissionAttribute.Options;
            var attrPerm = XDocument.Parse(userPermissionTree.GetAsXML());

            if (!XDocument.DeepEquals(currPerm, attrPerm))
            {
                int ret = userPermissionTree.Update();
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    string exceptionErr = String.Format("Erro atualizando Permissão {0} - {1}", permissionAttribute.PermissionID, err);
                    Logger.Error(exceptionErr);
                }
            }
        }

        private UserTableBOM FilterMissingTablesFromBOM(UserTableBOM bom)
        {
            int searchHead = 0;
            var ret = new UserTableBOM();
            int length = bom.With(x => x.BO).Return(x => x.Length, 0);

            if (length == 0)
                return ret;

            List<UserTableBOMBO> filtered = new List<UserTableBOMBO>();

            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                rs.DoQuery(CheckTablesSQL(bom));
                Array.Sort<UserTableBOMBO>(bom.BO);

                /**
                 * Using searchHead all search operation is done in O(n) of tables, at once.
                 */
                string tableName = "", resourceTableName = "";
                while (!rs.EoF)
                {
                    tableName = (string)rs.Fields.Item("TableName").Value;
                    resourceTableName = bom.BO[searchHead].With(x => x.UserTablesMD[0]).Return(x => x.TableName, String.Empty);
                    for (; searchHead < length && tableName != resourceTableName; searchHead++)
                        filtered.Add(bom.BO[searchHead]);

                    if (tableName == resourceTableName)
                        searchHead++;
                    rs.MoveNext();
                }

                for (; searchHead < length && tableName != resourceTableName; searchHead++)
                    filtered.Add(bom.BO[searchHead]);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }

            ret.BO = filtered.ToArray();
            return ret;
        }

        private string CheckTablesSQL(UserTableBOM bom)
        {
            StringBuilder sqlSb = new StringBuilder("select TableName from OUTB where OUTB.TableName in (");
            bool first = true;
            foreach (var table in bom.BO)
            {
                if (!first)
                    sqlSb.Append(", ");
                sqlSb.Append(String.Format("'{0}'", table.With(x => x.UserTablesMD[0])
                    .Return(x => x.TableName, String.Empty)));
                first = false;
            }
            sqlSb.Append(") order by TableName");
            return sqlSb.ToString();
        }

        private UserFieldBOM FilterMissingFieldsFromBOM(UserFieldBOM bom)
        {
            int searchHead = 0;
            var ret = new UserFieldBOM();
            int length = bom.With(x => x.BO).Return(x => x.Length, 0);

            if (length == 0)
                return ret;

            List<UserFieldBOMBO> filtered = new List<UserFieldBOMBO>();
            var fields = bom.BO;

            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                rs.DoQuery(CheckFieldsSQL(bom));
                Array.Sort<UserFieldBOMBO>(bom.BO);

                /**
                 * Using searchHead all search operation is done in O(n) of tables, at once.
                 */
                string fieldName = "", tableName = "", resourceFieldName = "", resourceTableName = "";
                while (!rs.EoF)
                {
                    fieldName = (string)rs.Fields.Item("AliasID").Value;
                    tableName = (string)rs.Fields.Item("TableID").Value;

                    resourceFieldName = fields[searchHead].With(x => x.UserFieldsMD[0]).Return(x => x.TableName, String.Empty);
                    resourceTableName = fields[searchHead].With(x => x.UserFieldsMD[0]).Return(x => x.TableName, String.Empty);

                    for (; searchHead < length && (fieldName != resourceFieldName || tableName != resourceTableName); searchHead++)
                        filtered.Add(fields[searchHead]);

                    if (tableName == resourceTableName && fieldName == resourceFieldName)
                        searchHead++;

                    rs.MoveNext();
                }
                for (; searchHead < length && (fieldName != resourceFieldName|| tableName != resourceTableName); searchHead++)
                    filtered.Add(fields[searchHead]);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }

            ret.BO = filtered.ToArray();
            return ret;
        }

        private string CheckFieldsSQL(UserFieldBOM fields)
        {
            StringBuilder sqlSb = new StringBuilder("select AliasID, TableID from CUFD where ");
            bool first = true;
            foreach (var field in fields.BO)
            {
                if (!first)
                    sqlSb.Append(" or ");
                sqlSb.Append(String.Format("(TableID = '{0}' and AliasID = '{1}')", field.UserFieldsMD[0].TableName,
                    field.UserFieldsMD[0].Name));
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
                try
                {
                    objUserField.Browser.ReadXml(xmlFields, i);
                    Logger.Info(String.Format("Criando campo [{0}].{1}", objUserField.TableName, objUserField.Name));
                    result = objUserField.Add();
                    if ((result != 0) && (result != -2035))
                    {
                        string errMsg = company.GetLastErrorDescription();
                        Logger.Error(String.Format("Erro ao criar campo de usuário[{0}].{1} {2}", objUserField.TableName,
                            objUserField.Name, errMsg));
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserField);
                }
            }

        }

        private void CreateTablesFromXML(string xmlTables)
        {
            int result;
            var objUserTable = (SAPbobsCOM.UserTablesMD)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
            try
            {
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
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserTable);
            }
        }

        public string GetNextCode(string udt)
        {
            SAPbobsCOM.Recordset objRecordSet = (SAPbobsCOM.Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string id;
            try
            {
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
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objRecordSet);
            }
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
                    if (!IsNotCoreType(type))
                    {
                        object obj = rs.Fields.Item(0).Value;
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
            Type type = typeof(T);
            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                rs.DoQuery(sql);
                int fieldsSize = rs.Fields.Count;
                while (!rs.EoF)
                {
                    T obj;
                    if (IsNotCoreType(type))
                        obj = PrepareObject<T>(rs);
                    else
                    {
                        if (rs.Fields.Item(0).Value.GetType() != type)
                        {
                            String errMsg = String.Format(
                                "Objeto do tipo {0}. Retorno da consulta no tipo {1}.",
                                rs.Fields.Item(0).Value.GetType(), type);
                            Logger.Error(errMsg);
                            throw new Exception(errMsg);
                        }
                        obj = (T)rs.Fields.Item(0).Value;
                    }
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
                    object value = rs.Fields.Item(i).Value;
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

        public T GetBOMFromXML<T>(Stream stream)
        {
            var deserializer = new XmlSerializer(typeof(T));
            var bom = (T)deserializer.Deserialize(stream);
            return bom;
        }

        public string GetUserTableXMLBOMFromNames(string[] userTables)
        {
            var ut = (IUserTablesMD)company.GetBusinessObject(BoObjectTypes.oUserTables);
            List<UserTableBOM> userTableBOMList = new List<UserTableBOM>();
            try
            {
                company.XMLAsString = true;
                company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;

                foreach (var table in userTables)
                {
                    if (!ut.GetByKey(table))
                        throw new Exception("Não foi possível encontrar a tabela " + table);

                    var xml = ut.GetAsXML();
                    userTableBOMList.Add(xml.Deserialize<UserTableBOM>());
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ut);
            }
            return userTableBOMList.Serialize<List<UserTableBOM>>();
        }

        public string GetUserFieldXMLBOMFromNames(string[] userTables)
        {
            var uf = (IUserFieldsMD)company.GetBusinessObject(BoObjectTypes.oUserFields);
            UserFieldBOM userFieldBOM = null, tmpBOM = null;
            Recordset rs = null;
            try
            {
                company.XMLAsString = true;
                company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;

                foreach (var table in userTables)
                {
                    try
                    {
                        rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
                        rs.DoQuery(string.Format(
                            "select TableID, FieldID from CUFD where TableID = '@{0}' order by FieldID", table));
                        uf.Browser.Recordset = rs;
                        while (!uf.Browser.EoF)
                        {
                            var xml = uf.GetAsXML();
                            tmpBOM = xml.Deserialize<UserFieldBOM>();
                            if (userFieldBOM == null)
                                userFieldBOM = tmpBOM;
                            else
                                userFieldBOM.BO = tmpBOM.BO.Concat(userFieldBOM.BO).ToArray();
                            uf.Browser.MoveNext();
                        }
                    }
                    finally
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                    }
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(uf);
            }
            return userFieldBOM.Serialize<UserFieldBOM>();
        }

    }
}
