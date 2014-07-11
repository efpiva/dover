using System.Linq;
using Dover.Framework.DAO;
using Dover.Framework.Service;
using SAPbouiCOM.Framework;

namespace Dover.Framework.Form
{
    public class DoverSystemFormBase : DoverOneFormBase
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
