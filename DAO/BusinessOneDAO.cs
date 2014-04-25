using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model.Assembly;
using AddOne.Framework.Model.SAP;
using System.IO;

namespace AddOne.Framework.DAO
{
    public interface BusinessOneDAO
    {

        void SaveBOMIfNotExists(UserTableBOM tables);

        void SaveBOMIfNotExists(UserFieldBOM fields);

        void UpdateOrSaveBOMIfNotExists(UDOBOM filteredUdoBOM);

        string GetNextCode(String udt);

        string GetCurrentUser();

        void ExecuteStatement(string sql);

        T ExecuteSqlForObject<T>(string sql);

        List<T> ExecuteSqlForList<T>(string sql);

        T GetBOMFromXML<T>(Stream resourceStream);

        string GetUserTableXMLBOMFromNames(string[] userTables);

        string GetUserFieldXMLBOMFromNames(string[] userTables);

        void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute);

        void UpdateOrSaveBOMIfNotExists(FormattedSearchBOM fsBOM);

        void UpdateOrSaveBOMIfNotExists(UserQueriesBOM qcBOM);

        void UpdateOrSaveBOMIfNotExists(QueryCategoriesBOM qcBOM);
    }
}
