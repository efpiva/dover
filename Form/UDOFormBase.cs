using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;
using SAPbouiCOM;

namespace AddOne.Framework.Form
{
    public class AddOneUDOFormBase : AddOneFormBase
    {
        private bool activated = false;

        public AddOneUDOFormBase()
        {
        }

        private string formUID;
        public BusinessOneUIDAO b1UIDAO { get; set; }

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
                this.OnInitializeComponent(); // UI elements displayed only after form creation.
                activated = true;
            }
        }

    }
}
