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
using Dover.Framework.Factory;
using Dover.Framework.Service;
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using Dover.Framework.DAO;
using Dover.Framework.Interface;

namespace Dover.Framework.Form
{
    /// <summary>
    /// Base class for UserForms.
    /// 
    /// It will read the resource name in FormAttribute and locate a form with
    /// the specified name in the B1S embedded resource. If not found, it will look for
    /// a srf XML file in the current solution and threat the specified resource name
    /// as the Fully Qualified Name of the embedded resource file.
    /// 
    /// </summary>
    public class DoverUserFormBase : DoverFormBase
    {

        public DoverUserFormBase()
        {
            var resourceManager = ContainerManager.Container.Resolve<B1SResourceManager>();
            var b1UIDAO = ContainerManager.Container.Resolve<BusinessOneUIDAO>();
            var formEventHandler = ContainerManager.Container.Resolve<IFormEventHandler>();
            
            FormAttribute formAttribute = (FormAttribute)(from attribute in this.GetType().GetCustomAttributes(true)
                                                            where attribute is FormAttribute
                                                            select attribute).First();
            var asmName = this.GetType().BaseType.Assembly.GetName().FullName;

            formEventHandler.RegisterFormLoadBefore(formAttribute.FormType, this);
            string xml = resourceManager.GetFormXML(asmName, formAttribute.Resource);
            this.UIAPIRawForm = b1UIDAO.LoadFormBatchAction(xml, formAttribute.FormType);
            if (this.UIAPIRawForm != null)
            {
                formEventHandler.RegisterForm(this.UIAPIRawForm.UniqueID, this);
                this.OnInitializeComponent();
            }
        }

    }
}
