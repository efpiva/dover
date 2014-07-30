/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
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
    public partial class FormattedSearchBOM : IBOM
    {
        private IBO[] boField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("BO", Type=typeof(FormattedSearchBOMBO))]
        public IBO[] BO
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class FormattedSearchBOMBO: IBO
    {
        private BOMBOAdmInfo admInfoField;

        private BOMBOQueryParams queryParamsField;

        private FormattedSearchField[] formattedSearchesField;

        private FormattedSearchValidValues[] userValidValuesField;

        internal override string[] GetKey()
        {
            return new string[]{"Index"};
        }

        internal override SAPbobsCOM.BoObjectTypes GetBOType()
        {
            return SAPbobsCOM.BoObjectTypes.oFormattedSearches;
        }

        internal override string GetName()
        {
            return Messages.FormattedSearch;
        }

        internal override System.Type GetBOClassType()
        {
            return typeof(SAPbobsCOM.IFormattedSearches);
        }

        internal override string GetFormattedKey()
        {
            return "[" +
                FormattedSearches
                    .With(x => x[0])
                    .Return(x => x.FormID, string.Empty)
                + "].[" +
                FormattedSearches
                    .With(x => x[0])
                    .Return(x => x.ItemID, string.Empty);
        }

        internal override string GetFormattedDescription()
        {
            return "[" +
                FormattedSearches
                    .With(x => x[0])
                    .Return(x => x.FormID, string.Empty)
                + "].[" +
                FormattedSearches
                    .With(x => x[0])
                    .Return(x => x.ItemID, string.Empty);
        }

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
        public FormattedSearchField[] FormattedSearches
        {
            get
            {
                return this.formattedSearchesField;
            }
            set
            {
                this.formattedSearchesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("row", IsNullable = false)]
        public FormattedSearchValidValues[] UserValidValues
        {
            get
            {
                return this.userValidValuesField;
            }
            set
            {
                this.userValidValuesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class FormattedSearchField
    {

        private string formIDField;

        private string itemIDField;

        private string columnIDField;

        private string actionField;

        private long queryIDField;

        private bool queryIDFieldSpecified;

        private string refreshField;

        private string fieldIDField;

        private string forceRefreshField;

        private string byFieldField;

        /// <remarks/>
        public string FormID
        {
            get
            {
                return this.formIDField;
            }
            set
            {
                this.formIDField = value;
            }
        }

        /// <remarks/>
        public string ItemID
        {
            get
            {
                return this.itemIDField;
            }
            set
            {
                this.itemIDField = value;
            }
        }

        /// <remarks/>
        public string ColumnID
        {
            get
            {
                return this.columnIDField;
            }
            set
            {
                this.columnIDField = value;
            }
        }

        /// <remarks/>
        public string Action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }

        /// <remarks/>
        public long QueryID
        {
            get
            {
                return this.queryIDField;
            }
            set
            {
                this.queryIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool QueryIDSpecified
        {
            get
            {
                return this.queryIDFieldSpecified;
            }
            set
            {
                this.queryIDFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string Refresh
        {
            get
            {
                return this.refreshField;
            }
            set
            {
                this.refreshField = value;
            }
        }

        /// <remarks/>
        public string FieldID
        {
            get
            {
                return this.fieldIDField;
            }
            set
            {
                this.fieldIDField = value;
            }
        }

        /// <remarks/>
        public string ForceRefresh
        {
            get
            {
                return this.forceRefreshField;
            }
            set
            {
                this.forceRefreshField = value;
            }
        }

        /// <remarks/>
        public string ByField
        {
            get
            {
                return this.byFieldField;
            }
            set
            {
                this.byFieldField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class FormattedSearchValidValues
    {

        private string fieldValueField;

        /// <remarks/>
        public string FieldValue
        {
            get
            {
                return this.fieldValueField;
            }
            set
            {
                this.fieldValueField = value;
            }
        }
    }
}