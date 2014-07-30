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
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using System.Threading;

namespace Dover.Framework.Form
{
    [FormAttribute("Dover.dbchange", "Dover.Framework.Form.DBChange.srf")]
    public class DBChange : DoverUserFormBase
    {
        internal DataTable DBChangeDT;
        internal Admin BaseForm { get; set; }
        private Button confirm, cancel;

        public override void OnInitializeComponent()
        {
            this.DBChangeDT = this.UIAPIRawForm.DataSources.DataTables.Item("dbchange");
            this.confirm = (Button)GetItem("btWarn").Specific;
            this.cancel = (Button)GetItem("btCancel").Specific;

            this.confirm.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(confirm_ClickAfter);
            this.cancel.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(cancel_ClickAfter);
        }

        protected virtual void confirm_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (BaseForm != null)
                BaseForm.InstallAddin();
            this.UIAPIRawForm.Close();
        }

        protected virtual void cancel_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            this.UIAPIRawForm.Close();
        }
    }
}
