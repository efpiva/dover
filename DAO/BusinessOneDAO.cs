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

    public abstract class BusinessOneDAO
    {
        public abstract void SaveBOMIfNotExists(IBOM bom, INotifier notifier = null);

        public abstract void UpdateOrSaveBOMIfNotExists(IBOM udoBOM, INotifier notifier = null);

        /// <summary>
        /// Return a BOM XML, containing various BO elements.
        /// </summary>
        /// <typeparam name="V">SAP Business One DI API object type</typeparam>
        /// <param name="keys">keys to be fetched</param>
        /// <param name="objType">SAP Business One DI API enum representing the object type</param>
        /// <returns></returns>
        public abstract string GetXMLBom<V>(object[] keys, BoObjectTypes objType);

        public abstract List<string> ListMissingBOMKeys(IBOM userFieldBOM);

        public abstract string GetNextCode(String udt);

        public abstract string GetCurrentUser();

        public abstract void ExecuteStatement(string sql);

        public abstract T ExecuteSqlForObject<T>(string sql);

        public abstract List<T> ExecuteSqlForList<T>(string sql);

        public abstract T GetBOMFromXML<T>(Stream resourceStream);

        public abstract void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute);

        public abstract bool IsSuperUser();

        public abstract void SaveBOM(IBOM doc, INotifier notifier = null);

        public abstract bool PermissionExists(Attribute.PermissionAttribute permissionAttribute);
    }
}
