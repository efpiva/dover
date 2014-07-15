using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using Dover.Framework.Factory;

namespace Dover.Framework.Form
{
    public class ExceptionTrace : DoverUserFormBase
    {

        private EditText exMessage;
        private EditText trace;
        private Button innerException;
        private Item innerItem;

        private Exception _ex;
        internal Exception ex 
        { 
            get
            {
                return _ex;
            }
            set 
            {
                _ex = value;
                exMessage.Value = _ex.Message;
                trace.Value = _ex.StackTrace.ToString();
                if (_ex.InnerException == null)
                    innerItem.Visible = false;
            }
        }

        public override void OnInitializeComponent()
        {
            exMessage = (EditText)this.GetItem("ex").Specific;
            trace = (EditText)this.GetItem("trace").Specific;
            innerItem = this.GetItem("InnerBt");
            innerException = (Button)innerItem.Specific;
            innerException.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(innerException_ClickAfter);
        }

        protected virtual void innerException_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (_ex.InnerException != null)
            {
                var traceForm = ContainerManager.Container.Resolve<ExceptionTrace>();
                traceForm.ex = _ex.InnerException;
                traceForm.Show();
            }
        }

    }
}
