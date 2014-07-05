using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using AddOne.Framework.Service;

namespace AddOne.Framework.Form
{
    public class AddOneUDOFormBase : AddOneFormBase
    {
        private bool activated = false;
        private string formUID;
        public BusinessOneUIDAO b1UIDAO { get; set; }
        public B1SResourceManager resourceManager { get; set; }

        public AddOneUDOFormBase()
        {
        }

        internal override string FormUID
        {
            set
            {
                this.formUID = value;
                this.UIAPIRawForm = b1UIDAO.GetFormByUID(formUID);
            }
        }

        protected internal override void OnFormActivateAfter(SBOItemEventArg pVal)
        {
            if (!activated)
            {
                try
                {
                    FormAttribute formAttribute = (FormAttribute)(from attribute in this.GetType().GetCustomAttributes(true)
                                                                  where attribute is FormAttribute
                                                                  select attribute).First();
                    var asmName = this.GetType().BaseType.Assembly.GetName().FullName;

                    string xml = resourceManager.GetSystemFormXML(asmName, formAttribute.Resource, formUID, this.UIAPIRawForm);
                    if (!string.IsNullOrEmpty(xml))
                        b1UIDAO.LoadBatchAction(xml);

                    this.OnInitializeComponent(); // UI elements displayed only after form creation.
                }
                finally // prevent loop on exceptions.
                {
                    activated = true;
                }
            }
        }

    }
}
