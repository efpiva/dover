using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.Model.SAP
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName="Application", Namespace = "", IsNullable = false)]
    public partial class UIApplication
    {

        private ApplicationMenus[] menus;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Menus", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ApplicationMenus[] Menus
        {
            get
            {
                return this.menus;
            }
            set
            {
                this.menus = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ApplicationMenus
    {

        private ApplicationMenusAction[] actionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("action", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ApplicationMenusAction[] action
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
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ApplicationMenusAction
    {

        private ApplicationMenusActionMenu[] menuField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Menu", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ApplicationMenusActionMenu[] Menu
        {
            get
            {
                return this.menuField;
            }
            set
            {
                this.menuField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
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
    }
}
