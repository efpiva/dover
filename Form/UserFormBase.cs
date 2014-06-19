
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Factory;
using AddOne.Framework.Service;
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using AddOne.Framework.DAO;

namespace AddOne.Framework.Form
{
    public class AddOneUserFormBase : AddOneFormBase
    {

        public AddOneUserFormBase()
        {
            var resourceManager = ContainerManager.Container.Resolve<B1SResourceManager>();
            var b1UIDAO = ContainerManager.Container.Resolve<BusinessOneUIDAO>();
            var formEventHandler = ContainerManager.Container.Resolve<FormEventHandler>();
            
            FormAttribute formAttribute = (FormAttribute)(from attribute in this.GetType().GetCustomAttributes(true)
                                                            where attribute is FormAttribute
                                                            select attribute).First();
            var asmName = this.GetType().Assembly.GetName().FullName;

            formEventHandler.RegisterFormLoadBefore(formAttribute.FormType, this);
            string xml = resourceManager.GetFormXML(asmName, formAttribute.Resource);
            this.UIAPIRawForm = b1UIDAO.LoadFormBatchAction(xml);
            formEventHandler.RegisterForm(this.UIAPIRawForm.UniqueID, this);
            this.OnInitializeComponent();
        }

    }
}
