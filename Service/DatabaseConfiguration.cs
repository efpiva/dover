using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;
using AddOne.Framework.Model.SAP;
using System.Reflection;
using System.Xml.Serialization;

namespace AddOne.Framework.Service
{
    public class DatabaseConfiguration
    {
        private BusinessOneDAO b1DAO;
        private const string DBTABLES_XML = "AddOne.Framework.DatabaseTables.xml";
        private const string DBFIELDS_XML = "AddOne.Framework.DatabaseFields.xml";

        public DatabaseConfiguration(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        public void PrepareDatabase()
        {
            UserTableBOM tables;
            UserFieldBOM fields;

            using (var tableStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DBTABLES_XML))
            using (var fieldStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DBFIELDS_XML))
            {
                tables = b1DAO.GetBOMFromXML<UserTableBOM>(tableStream);
                fields = b1DAO.GetBOMFromXML<UserFieldBOM>(fieldStream);
            }

            b1DAO.SaveBOMIfNotExists(tables);
            b1DAO.SaveBOMIfNotExists(fields);
        }
    }
}
