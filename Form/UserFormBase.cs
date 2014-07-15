
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Factory;
using Dover.Framework.Service;
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using Dover.Framework.DAO;

namespace Dover.Framework.Form
{
    public class DoverUserFormBase : DoverFormBase
    {

        public DoverUserFormBase()
        {
            var resourceManager = ContainerManager.Container.Resolve<B1SResourceManager>();
            var b1UIDAO = ContainerManager.Container.Resolve<BusinessOneUIDAO>();
            var formEventHandler = ContainerManager.Container.Resolve<FormEventHandler>();
            
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
