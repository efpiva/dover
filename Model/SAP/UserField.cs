using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using System.Xml.Serialization;

namespace AddOne.Framework.Model.SAP
{

    [XmlRoot("BOM")]
    public class GAUserFieldBOM
    {
        [XmlElement("BO")]
        public List<GAUserFieldMD> BO;
    }

    public class GAUserFieldMD : BO
    {

        [XmlArray("UserFieldsMD")]
        [XmlArrayItem("row")]
        public List<GAUserField> UserFieldMD = new List<GAUserField>();
        [XmlArray("ValidValuesMD")]
        [XmlArrayItem("row")]
        public List<GAValidValues> ValidValuesMD;

    }

    [XmlRoot("row")]
    public class GAUserField : IComparable<GAUserField>
    {
        private string tableName;
        private string name;
        private string description;
        private int editSize;

        public BoFieldTypes Type;
        public BoFldSubTypes SubType = BoFldSubTypes.st_None;
        public string DefaultValue = "";
        public BoYesNoEnum Mandatory = BoYesNoEnum.tNO;
        public string LinkedTable = "";
        [XmlIgnore]
        public List<GAValidValues> ValidValuesMD = new List<GAValidValues>();

        public GAUserField() { }

        public GAUserField(string fieldName, string description, BoFieldTypes type, int size, BoFldSubTypes subType,
            string tableName = "", string validValues = "", BoYesNoEnum mandatory = BoYesNoEnum.tNO, string linkedTable = "")
        {
            this.TableName = tableName;
            this.Name = fieldName;
            this.Description = description;
            this.Type = type;
            this.SubType = subType;
            this.editSize = size;
            this.DefaultValue = validValues;
            this.Mandatory = mandatory;
            this.LinkedTable = linkedTable;
        }

        public string TableName
        {
            get { return tableName; }
            set
            {
                if (value != null && value.Length > 20)
                    throw new Exception("Table name longer than 20");
                this.tableName = value;
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value != null && value.Length > 18)
                    throw new Exception("Name longer than 18");
                this.name = value;
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (value != null && value.Length > 30)
                    throw new Exception("Description longer than 30");
                this.description = value;
            }
        }

        public String EditSize
        {
            get { return (editSize > 0) ? editSize.ToString() : ""; }
            set { Int32.TryParse(value, out editSize); }
        }

        public int CompareTo(GAUserField other)
        {
            if (other == null)
                return -1;

            int tableCompare = this.TableName.CompareTo(other.TableName);
            if (tableCompare == 0)
                return this.Name.CompareTo(other.Name);
            else
                return tableCompare;
        }

    }
}
