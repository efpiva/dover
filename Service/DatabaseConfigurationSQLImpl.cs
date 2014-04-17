using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;
using AddOne.Framework.Model.SAP;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace AddOne.Framework.Service
{
    class DatabaseConfigurationSQLImpl : DatabaseConfiguration
    {
        private BusinessOneDAO b1DAO;
        private const string DATABASE_XML = "AddOne.Framework.DatabaseTables.xml";

        public DatabaseConfigurationSQLImpl(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        public void PrepareDatabase()
        {
            List<GAUserTable> tables = GetDatabaseTables();
            List<GAUserTable> tablesToCreate = b1DAO.CheckTablesExists(tables);
            List<GAUserField> fieldsToCreate = b1DAO.CheckFieldsExists(tables);

            b1DAO.CreateTables(tablesToCreate);
            b1DAO.CreateFields(fieldsToCreate);
        }

        private List<GAUserTable> GetDatabaseTables()
        {

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DATABASE_XML))
            {
                var deserializer = new XmlSerializer(typeof(List<GAUserTable>));
                return (List<GAUserTable>)deserializer.Deserialize(stream);
            }
        }
    }
}
