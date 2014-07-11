using System.Xml.Serialization;
using Dover.Framework.Monad;

namespace Dover.Framework.Model.SAP
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "BOM", Namespace = "", IsNullable = false)]
    public partial class UserQueriesBOM : IBOM
    {

        private UserQueriesBOMBO[] boField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BO")]
        public UserQueriesBOMBO[] BO
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
            return new string[] { "InternalKey", "QueryCategory" };
        }

        internal override SAPbobsCOM.BoObjectTypes GetBOType()
        {
            return SAPbobsCOM.BoObjectTypes.oUserQueries;
        }

        internal override System.Type GetBOClassType()
        {
            return typeof(SAPbobsCOM.IUserQueries);
        }

        internal override string GetName()
        {
            return Messages.UserQuery;
        }

        internal override string GetFormatName(int i)
        {
            return "[" + boField.With(x => x[i])
                .With(x => x.UserQueries)
                .With(x => x[0])
                .Return(x => x.QueryDescription, string.Empty) + "]";
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UserQueriesBOMBO
    {

        private BOMBOAdmInfo admInfoField;

        private BOMBOQueryParams queryParamsField;

        private UserQueriesField[] userQueriesField;

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
        public UserQueriesField[] UserQueries
        {
            get
            {
                return this.userQueriesField;
            }
            set
            {
                this.userQueriesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UserQueriesField
    {

        private long queryCategoryField;

        private bool queryCategoryFieldSpecified;

        private string queryDescriptionField;

        private string queryField;

        /// <remarks/>
        public long QueryCategory
        {
            get
            {
                return this.queryCategoryField;
            }
            set
            {
                this.queryCategoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QueryCategorySpecified
        {
            get
            {
                return this.queryCategoryFieldSpecified;
            }
            set
            {
                this.queryCategoryFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string QueryDescription
        {
            get
            {
                return this.queryDescriptionField;
            }
            set
            {
                this.queryDescriptionField = value;
            }
        }

        /// <remarks/>
        public string Query
        {
            get
            {
                return this.queryField;
            }
            set
            {
                this.queryField = value;
            }
        }
    }
}