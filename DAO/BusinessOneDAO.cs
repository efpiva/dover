using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Model;
using Dover.Framework.Model.SAP;
using System.IO;
using SAPbobsCOM;

namespace Dover.Framework.DAO
{
    public interface INotifier
    {
        void OnSuccess(string xml, string addedKey);
        void OnError(string xml, int errCode, string errMessage);
    }

    public interface BusinessOneDAO
    {
        void SaveBOMIfNotExists(IBOM bom, INotifier notifier = null);

        void UpdateOrSaveBOMIfNotExists(IBOM udoBOM, INotifier notifier = null);

        /// <summary>
        /// Return a BOM XML, containing various BO elements.
        /// </summary>
        /// <typeparam name="V">SAP Business One DI API object type</typeparam>
        /// <param name="keys">keys to be fetched</param>
        /// <param name="objType">SAP Business One DI API enum representing the object type</param>
        /// <returns></returns>
        string GetXMLBom<V>(object[] keys, BoObjectTypes objType);

        List<string> ListMissingBOMKeys(IBOM userFieldBOM);

        string GetNextCode(String udt);

        string GetCurrentUser();

        void ExecuteStatement(string sql);

        T ExecuteSqlForObject<T>(string sql);

        List<T> ExecuteSqlForList<T>(string sql);

        T GetBOMFromXML<T>(Stream resourceStream);

        void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute);

        bool IsSuperUser();

        void SaveBOM(IBOM doc, INotifier notifier = null);

        bool PermissionExists(Attribute.PermissionAttribute permissionAttribute);
    }
}
