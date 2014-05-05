using System.Xml.Serialization;
using AddOne.Framework.Monad;

namespace AddOne.Framework.Model.SAP
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "BOM", Namespace = "", IsNullable = false)]
    public class UDOBOM : IBOM
    {

        private UDOBOMBO[] boField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BO")]
        public UDOBOMBO[] BO
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
            return new string[] { "Code" };
        }

        internal override SAPbobsCOM.BoObjectTypes GetBOType()
        {
            return SAPbobsCOM.BoObjectTypes.oUserObjectsMD;
        }

        internal override System.Type GetBOClassType()
        {
            return typeof(SAPbobsCOM.IUserObjectsMD);
        }

        internal override string GetName()
        {
            return Messages.UDO;
        }

        internal override string GetFormatName(int i)
        {
            return "[" + boField.With(x => x[i])
                .With(x => x.UserObjectsMD)
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
    public partial class UDOBOMBO
    {

        private BOMBOAdmInfo admInfoField;

        private BOMBOQueryParams queryParamsField;

        private BOMBOField[] userObjectsMDField;

        private BOMBOChildTablesField[] userObjectMD_ChildTablesField;

        private BOMBOFindColumnsField[] userObjectMD_FindColumnsField;

        private BOMBOFormColumnsField[] userObjectMD_FormColumnsField;

        private BOMBOEnhancedFormColumnsField[] userObjectMD_EnhancedFormColumnsField;

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
        public BOMBOField[] UserObjectsMD
        {
            get
            {
                return this.userObjectsMDField;
            }
            set
            {
                this.userObjectsMDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public BOMBOChildTablesField[] UserObjectMD_ChildTables
        {
            get
            {
                return this.userObjectMD_ChildTablesField;
            }
            set
            {
                this.userObjectMD_ChildTablesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public BOMBOFindColumnsField[] UserObjectMD_FindColumns
        {
            get
            {
                return this.userObjectMD_FindColumnsField;
            }
            set
            {
                this.userObjectMD_FindColumnsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public BOMBOFormColumnsField[] UserObjectMD_FormColumns
        {
            get
            {
                return this.userObjectMD_FormColumnsField;
            }
            set
            {
                this.userObjectMD_FormColumnsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public BOMBOEnhancedFormColumnsField[] UserObjectMD_EnhancedFormColumns
        {
            get
            {
                return this.userObjectMD_EnhancedFormColumnsField;
            }
            set
            {
                this.userObjectMD_EnhancedFormColumnsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BOMBOField
    {

        private string codeField;

        private string nameField;

        private string tableNameField;

        private string logTableNameField;

        private string objectTypeField;

        private string manageSeriesField;

        private string canDeleteField;

        private string canCloseField;

        private string canCancelField;

        private string extensionNameField;

        private string canFindField;

        private string canYearTransferField;

        private string canCreateDefaultFormField;

        private string canLogField;

        private string overwriteDllfileField;

        private string useUniqueFormTypeField;

        private string canArchiveField;

        private string menuItemField;

        private string menuCaptionField;

        private long fatherMenuIDField;

        private bool fatherMenuIDFieldSpecified;

        private long positionField;

        private bool positionFieldSpecified;

        private string enableEnhancedFormField;

        private string rebuildEnhancedFormField;

        private string formSRFField;

        private string menuUIDField;

        /// <remarks/>
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

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
        public string LogTableName
        {
            get
            {
                return this.logTableNameField;
            }
            set
            {
                this.logTableNameField = value;
            }
        }

        /// <remarks/>
        public string ObjectType
        {
            get
            {
                return this.objectTypeField;
            }
            set
            {
                this.objectTypeField = value;
            }
        }

        /// <remarks/>
        public string ManageSeries
        {
            get
            {
                return this.manageSeriesField;
            }
            set
            {
                this.manageSeriesField = value;
            }
        }

        /// <remarks/>
        public string CanDelete
        {
            get
            {
                return this.canDeleteField;
            }
            set
            {
                this.canDeleteField = value;
            }
        }

        /// <remarks/>
        public string CanClose
        {
            get
            {
                return this.canCloseField;
            }
            set
            {
                this.canCloseField = value;
            }
        }

        /// <remarks/>
        public string CanCancel
        {
            get
            {
                return this.canCancelField;
            }
            set
            {
                this.canCancelField = value;
            }
        }

        /// <remarks/>
        public string ExtensionName
        {
            get
            {
                return this.extensionNameField;
            }
            set
            {
                this.extensionNameField = value;
            }
        }

        /// <remarks/>
        public string CanFind
        {
            get
            {
                return this.canFindField;
            }
            set
            {
                this.canFindField = value;
            }
        }

        /// <remarks/>
        public string CanYearTransfer
        {
            get
            {
                return this.canYearTransferField;
            }
            set
            {
                this.canYearTransferField = value;
            }
        }

        /// <remarks/>
        public string CanCreateDefaultForm
        {
            get
            {
                return this.canCreateDefaultFormField;
            }
            set
            {
                this.canCreateDefaultFormField = value;
            }
        }

        /// <remarks/>
        public string CanLog
        {
            get
            {
                return this.canLogField;
            }
            set
            {
                this.canLogField = value;
            }
        }

        /// <remarks/>
        public string OverwriteDllfile
        {
            get
            {
                return this.overwriteDllfileField;
            }
            set
            {
                this.overwriteDllfileField = value;
            }
        }

        /// <remarks/>
        public string UseUniqueFormType
        {
            get
            {
                return this.useUniqueFormTypeField;
            }
            set
            {
                this.useUniqueFormTypeField = value;
            }
        }

        /// <remarks/>
        public string CanArchive
        {
            get
            {
                return this.canArchiveField;
            }
            set
            {
                this.canArchiveField = value;
            }
        }

        /// <remarks/>
        public string MenuItem
        {
            get
            {
                return this.menuItemField;
            }
            set
            {
                this.menuItemField = value;
            }
        }

        /// <remarks/>
        public string MenuCaption
        {
            get
            {
                return this.menuCaptionField;
            }
            set
            {
                this.menuCaptionField = value;
            }
        }

        /// <remarks/>
        public long FatherMenuID
        {
            get
            {
                return this.fatherMenuIDField;
            }
            set
            {
                this.fatherMenuIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool FatherMenuIDSpecified
        {
            get
            {
                return this.fatherMenuIDFieldSpecified;
            }
            set
            {
                this.fatherMenuIDFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long Position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool PositionSpecified
        {
            get
            {
                return this.positionFieldSpecified;
            }
            set
            {
                this.positionFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string EnableEnhancedForm
        {
            get
            {
                return this.enableEnhancedFormField;
            }
            set
            {
                this.enableEnhancedFormField = value;
            }
        }

        /// <remarks/>
        public string RebuildEnhancedForm
        {
            get
            {
                return this.rebuildEnhancedFormField;
            }
            set
            {
                this.rebuildEnhancedFormField = value;
            }
        }

        /// <remarks/>
        public string FormSRF
        {
            get
            {
                return this.formSRFField;
            }
            set
            {
                this.formSRFField = value;
            }
        }

        /// <remarks/>
        public string MenuUID
        {
            get
            {
                return this.menuUIDField;
            }
            set
            {
                this.menuUIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BOMBOChildTablesField
    {

        private string tableNameField;

        private string logTableNameField;

        private string objectNameField;

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
        public string LogTableName
        {
            get
            {
                return this.logTableNameField;
            }
            set
            {
                this.logTableNameField = value;
            }
        }

        /// <remarks/>
        public string ObjectName
        {
            get
            {
                return this.objectNameField;
            }
            set
            {
                this.objectNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BOMBOFindColumnsField
    {

        private string columnAliasField;

        private string columnDescriptionField;

        /// <remarks/>
        public string ColumnAlias
        {
            get
            {
                return this.columnAliasField;
            }
            set
            {
                this.columnAliasField = value;
            }
        }

        /// <remarks/>
        public string ColumnDescription
        {
            get
            {
                return this.columnDescriptionField;
            }
            set
            {
                this.columnDescriptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BOMBOFormColumnsField
    {

        private long sonNumberField;

        private bool sonNumberFieldSpecified;

        private string formColumnAliasField;

        private string formColumnDescriptionField;

        private string editableField;

        /// <remarks/>
        public long SonNumber
        {
            get
            {
                return this.sonNumberField;
            }
            set
            {
                this.sonNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SonNumberSpecified
        {
            get
            {
                return this.sonNumberFieldSpecified;
            }
            set
            {
                this.sonNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string FormColumnAlias
        {
            get
            {
                return this.formColumnAliasField;
            }
            set
            {
                this.formColumnAliasField = value;
            }
        }

        /// <remarks/>
        public string FormColumnDescription
        {
            get
            {
                return this.formColumnDescriptionField;
            }
            set
            {
                this.formColumnDescriptionField = value;
            }
        }

        /// <remarks/>
        public string Editable
        {
            get
            {
                return this.editableField;
            }
            set
            {
                this.editableField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class BOMBOEnhancedFormColumnsField
    {

        private long columnNumberField;

        private bool columnNumberFieldSpecified;

        private long childNumberField;

        private bool childNumberFieldSpecified;

        private string columnAliasField;

        private string columnDescriptionField;

        private string columnIsUsedField;

        private string editableField;

        /// <remarks/>
        public long ColumnNumber
        {
            get
            {
                return this.columnNumberField;
            }
            set
            {
                this.columnNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ColumnNumberSpecified
        {
            get
            {
                return this.columnNumberFieldSpecified;
            }
            set
            {
                this.columnNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public long ChildNumber
        {
            get
            {
                return this.childNumberField;
            }
            set
            {
                this.childNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ChildNumberSpecified
        {
            get
            {
                return this.childNumberFieldSpecified;
            }
            set
            {
                this.childNumberFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string ColumnAlias
        {
            get
            {
                return this.columnAliasField;
            }
            set
            {
                this.columnAliasField = value;
            }
        }

        /// <remarks/>
        public string ColumnDescription
        {
            get
            {
                return this.columnDescriptionField;
            }
            set
            {
                this.columnDescriptionField = value;
            }
        }

        /// <remarks/>
        public string ColumnIsUsed
        {
            get
            {
                return this.columnIsUsedField;
            }
            set
            {
                this.columnIsUsedField = value;
            }
        }

        /// <remarks/>
        public string Editable
        {
            get
            {
                return this.editableField;
            }
            set
            {
                this.editableField = value;
            }
        }
    }
}

