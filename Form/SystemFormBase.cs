using System.Linq;
using AddOne.Framework.DAO;
using AddOne.Framework.Service;
using SAPbouiCOM.Framework;

namespace AddOne.Framework.Form
{
    public class AddOneSystemFormBase : AddOneFormBase
    {
        private string formUID;
        public B1SResourceManager resourceManager { get; set; }
        public BusinessOneUIDAO b1UIDAO { get; set; }
        public FormEventHandler formEventHandler { get; set; }

        public AddOneSystemFormBase()
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
            var asmName = this.GetType().Assembly.GetName().FullName;

            string xml = resourceManager.GetSystemFormXML(asmName, formAttribute.Resource, formUID, this.UIAPIRawForm);
            b1UIDAO.LoadBatchAction(xml);
            formEventHandler.RegisterForm(formUID, this);
            this.OnInitializeComponent();
        }

    }
}
