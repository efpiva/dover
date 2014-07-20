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
using System.Linq;
using Dover.Framework.DAO;
using Dover.Framework.Service;
using SAPbouiCOM.Framework;

namespace Dover.Framework.Form
{
    public class DoverSystemFormBase : DoverFormBase
    {
        private string formUID;
        public B1SResourceManager resourceManager { get; set; }
        public BusinessOneUIDAO b1UIDAO { get; set; }
        public FormEventHandler formEventHandler { get; set; }

        public DoverSystemFormBase()
        {
        }

        internal override string FormUID
        {
            set
            {
                this.formUID = value;
                this.UIAPIRawForm = b1UIDAO.GetFormByUID(formUID);
                UpdateSystemForm();
            }
        }

        private void UpdateSystemForm()
        {
            FormAttribute formAttribute = (FormAttribute)(from attribute in this.GetType().GetCustomAttributes(true)
                                                          where attribute is FormAttribute
                                                          select attribute).First();
            var asmName = this.GetType().BaseType.Assembly.GetName().FullName;

            string xml = resourceManager.GetSystemFormXML(asmName, formAttribute.Resource, formUID);
            b1UIDAO.LoadBatchAction(xml);
            formEventHandler.RegisterForm(formUID, this);
            this.OnInitializeComponent();
        }

    }
}
