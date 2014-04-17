using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using System.Xml.Serialization;
using System.Xml;

namespace AddOne.Framework.Model.SAP
{

    [XmlRoot("BOM")]
    public class GAUserTableBOM
    {
        [XmlElement("BO")]
        public List<GAUserTableMD> BO;
    }

    public class GAUserTableMD : BO
    {

        [XmlArray("UserTablesMD")]
        [XmlArrayItem("row")]
        public List<GAUserTable> UserTableMD = new List<GAUserTable>();
    }

    [XmlRoot("row")]
    public class GAUserTable : IComparable<GAUserTable>
    {
        public BoUTBTableType TableType;
        private string tableName;
        private string tableDescription;

        public List<GAUserField> UserFields = null;
        private XmlSerializerNamespaces _namespaces;


        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces
        {
            get { return this._namespaces; }
        }

        public GAUserTable() { }

        public GAUserTable(string name, string description, BoUTBTableType type)
        {
            this.TableName = name;
            this.TableDescription = description;
            this.TableType = type;
        }

        public string TableName
        {
            get { return tableName; }
            set
            {
                if (value != null && value.Length > 19)
                    throw new Exception("Table name longer than 19");
                this.tableName = value;
            }
        }

        public string TableDescription
        {
            get { return tableDescription; }
            set
            {
                if (value != null && value.Length > 30)
                    throw new Exception("Table description longer than 30");
                this.tableDescription = value;
            }
        }

        public int CompareTo(GAUserTable other)
        {
            if (other == null)
                return -1;

            return this.TableName.CompareTo(other.TableName);
        }
    }
}
