using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using Dover.Framework.Attribute;
using Castle.Core.Logging;
using SAPbouiCOM;
using Dover.Framework.Service;

namespace Dover.Framework.Form
{
    [MenuEvent(UniqueUID = "doverLicense")]
    [FormAttribute("Dover.license", "Dover.Framework.Form.License.srf")]
    public class License : DoverUserFormBase
    {
        public ILogger Logger { get; set; }

        private Button fileButton;
        private Button installButton;
        private DataTable licenseDT;
        private DataTable emptyDT;
        private EditText modulePath;

        private LicenseManager _licenseManager;
        public LicenseManager licenseManager { 
            get 
            { 
                return _licenseManager;
            }
            set
            {
                _licenseManager = value;
                updateLicenseDT();
            }
        }

        public License()
        {
        }

        public override void OnInitializeComponent()
        {
            fileButton = (Button)this.GetItem("fileBt").Specific;
            installButton = (Button)this.GetItem("importBt").Specific;
            modulePath = (EditText)this.GetItem("moduleEd").Specific;
            licenseDT = this.UIAPIRawForm.DataSources.DataTables.Item("licenseDT");
            emptyDT = this.UIAPIRawForm.DataSources.DataTables.Item("emptyDT");

            fileButton.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(fileButton_ClickAfter);
            installButton.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(installButton_ClickAfter);
        }


        protected virtual void fileButton_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            SelectFileDialog dialog = new SelectFileDialog("C:\\", "",
                Messages.AdminModuleFilterPrefix + "|*.xml", DialogType.OPEN);
            dialog.Open();
            modulePath.Value = dialog.SelectedFile;
        }

        protected virtual void installButton_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (string.IsNullOrWhiteSpace(modulePath.Value))
            {
                Logger.Error(Messages.AdminEmptyFile);
                return;
            }

            if (licenseManager.SaveLicense(modulePath.Value))
            {
                updateLicenseDT();
                Logger.Info(Messages.LicenseSuccessInstall);
            }
            else
            {
                Logger.Error(Messages.LicenseErrorInstall);
            }
        }

        private void updateLicenseDT()
        {
            this.UIAPIRawForm.Freeze(true);

            var modulesList = licenseManager.ListAddins();
            licenseDT.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly, emptyDT.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly));
            for (int i = 0; i < modulesList.Count; ++i )
            {
                var module = modulesList[i];
                licenseDT.Rows.Add();
                licenseDT.SetValue("Name", i, module.Name);
                licenseDT.SetValue("Description", i, module.Description);
                DateTime dueDate = licenseManager.GetAddInExpireDate(module.Name);
                string dueDateStr = (dueDate == DateTime.MinValue) ? Messages.LicenseEmpty : dueDate.ToShortDateString();
                licenseDT.SetValue("DueDate", i, dueDateStr);
            }
            this.UIAPIRawForm.Freeze(false);
        }
    }
}
