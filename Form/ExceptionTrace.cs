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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using Dover.Framework.Factory;
using Dover.Framework.Attribute;

namespace Dover.Framework.Form
{
    [DoverForm("dover.exception", "Dover.Framework.Form.ExceptionTrace.srf")]
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
