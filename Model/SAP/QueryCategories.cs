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
    public partial class QueryCategoriesBOM : IBOM
    {

        private QueryCategoriesBOMBO[] boField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BO")]
        public QueryCategoriesBOMBO[] BO
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
            return SAPbobsCOM.BoObjectTypes.oQueryCategories;
        }

        internal override System.Type GetBOClassType()
        {
            return typeof(SAPbobsCOM.IQueryCategories);
        }

        internal override string GetName()
        {
            return Messages.QueryCategory;
        }

        internal override string GetFormatName(int i)
        {
            return "[" + boField.With(x => x[i])
                .With(x => x.QueryCategories)
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
    public partial class QueryCategoriesBOMBO
    {

        private BOMBOAdmInfo admInfoField;

        private BOMBOQueryParams queryParamsField;

        private QueryCategoriesField[] queryCategoriesField;

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
        public QueryCategoriesField[] QueryCategories
        {
            get
            {
                return this.queryCategoriesField;
            }
            set
            {
                this.queryCategoriesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QueryCategoriesField
    {

        private string nameField;

        private string permissionsField;

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
        public string Permissions
        {
            get
            {
                return this.permissionsField;
            }
            set
            {
                this.permissionsField = value;
            }
        }
    }
}