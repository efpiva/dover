using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model.Assembly;
using AddOne.Framework.Model.SAP;
using System.IO;
using SAPbobsCOM;

namespace AddOne.Framework.DAO
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

        public abstract string GetNextCode(String udt);

        public abstract string GetCurrentUser();

        public abstract void ExecuteStatement(string sql);

        public abstract T ExecuteSqlForObject<T>(string sql);

        public abstract List<T> ExecuteSqlForList<T>(string sql);

        public abstract T GetBOMFromXML<T>(Stream resourceStream);

        public abstract string GetUserTableXMLBOMFromNames(string[] userTables);

        public abstract string GetUserFieldXMLBOMFromNames(string[] userTables);

        public abstract void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute);

        public abstract bool IsSuperUser();

        public abstract bool HasPermission(string permissionID);

        public abstract void SaveBOM(IBOM doc, INotifier notifier = null);
    }
}
