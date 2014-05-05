using System.Xml.Serialization;
using SAPbobsCOM;
using System;
using AddOne.Framework.Monad;

namespace AddOne.Framework.Model.SAP
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName="BOM", Namespace = "", IsNullable = false)]
    public class UserFieldBOM : IBOM
    {

        private UserFieldBOMBO[] boField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BO")]
        public UserFieldBOMBO[] BO
        {
            get
            {
                return this.boField;
            }
            set
            {
                this.boField = value;
            }
        }

        internal override string[] GetKey()
        {
            return new string[] { "TableName", "Name" };
        }

        internal override BoObjectTypes GetBOType()
        {
            return BoObjectTypes.oUserFields;
        }

        internal override Type GetBOClassType()
        {
            return typeof(IUserFieldsMD);
        }

        internal override string GetName()
        {
            return Messages.UDF;
        }

        internal override string GetFormatName(int i)
        {
            return "[" + boField.With(x => x[i])
                .With(x => x.UserFieldsMD)
                .With(x => x[0])
                .Return(x => x.TableName, string.Empty) + "].["
                +
                boField.With(x => x[i])
                .With(x => x.UserFieldsMD)
                .With(x => x[0])
                .Return(x => x.Name, string.Empty) + "]";
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UserFieldBOMBO : IComparable<UserFieldBOMBO>
    {

        private BOMBOAdmInfo admInfoField;

        private BOMBOQueryParams queryParamsField;

        private UserField[] userFieldsMDField;

        private ValidValues[] validValuesMDField;

        /// <remarks/>
        public BOMBOAdmInfo AdmInfo
        {
            get
            {
                return this.admInfoField;
            }
            set
            {
                this.admInfoField = value;
            }
        }

        /// <remarks/>
        public BOMBOQueryParams QueryParams
        {
            get
            {
                return this.queryParamsField;
            }
            set
            {
                this.queryParamsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public UserField[] UserFieldsMD
        {
            get
            {
                return this.userFieldsMDField;
            }
            set
            {
                this.userFieldsMDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public ValidValues[] ValidValuesMD
        {
            get
            {
                return this.validValuesMDField;
            }
            set
            {
                this.validValuesMDField = value;
            }
        }

        public int CompareTo(UserFieldBOMBO other)
        {
            if (this.userFieldsMDField != null && this.userFieldsMDField.Length == 1
                && other.userFieldsMDField != null && other.userFieldsMDField.Length == 1)
            {
                if (this.userFieldsMDField[0].TableName == other.userFieldsMDField[0].TableName)
                    return this.userFieldsMDField[0].Name.CompareTo(other.userFieldsMDField[0].Name);
                else
                    return this.userFieldsMDField[0].TableName.CompareTo(other.userFieldsMDField[0].TableName);
            }
            return -1;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UserField
    {

        private string nameField;

        private BoFieldTypes typeField;

        private long sizeField;

        private bool sizeFieldSpecified;

        private string descriptionField;

        private BoFldSubTypes subTypeField = BoFldSubTypes.st_None;

        private string linkedTableField = "";

        private string defaultValueField = "";

        private string tableNameField;

        private long editSizeField;

        private bool editSizeFieldSpecified;

        private BoYesNoEnum mandatoryField = BoYesNoEnum.tNO;

        private string linkedUDOField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public BoFieldTypes Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public long Size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SizeSpecified
        {
            get
            {
                return this.sizeFieldSpecified;
            }
            set
            {
                this.sizeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public BoFldSubTypes SubType
        {
            get
            {
                return this.subTypeField;
            }
            set
            {
                this.subTypeField = value;
            }
        }

        /// <remarks/>
        public string LinkedTable
        {
            get
            {
                return this.linkedTableField;
            }
            set
            {
                this.linkedTableField = value;
            }
        }

        /// <remarks/>
        public string DefaultValue
        {
            get
            {
                return this.defaultValueField;
            }
            set
            {
                this.defaultValueField = value;
            }
        }

        /// <remarks/>
        public string TableName
        {
            get
            {
                return this.tableNameField;
            }
            set
            {
                this.tableNameField = value;
            }
        }

        /// <remarks/>
        public long EditSize
        {
            get
            {
                return this.editSizeField;
            }
            set
            {
                this.editSizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EditSizeSpecified
        {
            get
            {
                return this.editSizeFieldSpecified;
            }
            set
            {
                this.editSizeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public BoYesNoEnum Mandatory
        {
            get
            {
                return this.mandatoryField;
            }
            set
            {
                this.mandatoryField = value;
            }
        }

        /// <remarks/>
        public string LinkedUDO
        {
            get
            {
                return this.linkedUDOField;
            }
            set
            {
                this.linkedUDOField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ValidValues
    {

        private string valueField;

        private string descriptionField;

        /// <remarks/>
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }
}
