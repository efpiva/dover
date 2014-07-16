using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using Dover.Framework.Attribute;
using Dover.Framework.Service;
using Castle.Core.Logging;
using SAPbouiCOM;
using Dover.Framework.Form;
using Dover.Framework.Log;

namespace Dover.Framework.Form
{

    [MenuEvent(UniqueUID = "doverAdmin")]
    [FormAttribute("dover.formAdmin", "Dover.Framework.Form.Admin.srf")]
    public class Admin : DoverUserFormBase
    {
        private DataTable removeDT;
        private DataTable installDT;
        private DataTable configTemp;

        private AssemblyManager asmLoader;
        private ILogger Logger;
        private LicenseManager licenseManager;

        // UI components
        private SAPbouiCOM.EditText modulePath;
        private SAPbouiCOM.Button installUpdateModule;
        private SAPbouiCOM.Button fileSelector;
        private SAPbouiCOM.Grid moduleGrid;
        private SAPbouiCOM.Grid removeGrid;
        private SAPbouiCOM.Button removeButtom;

        public Admin(AssemblyManager asmLoader, LicenseManager licenseManager, ILogger logger)
        {
            this.asmLoader = asmLoader;
            this.Logger = logger;
            this.licenseManager = licenseManager;
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.fileSelector = ((SAPbouiCOM.Button)(this.GetItem("btArq").Specific));
            this.fileSelector.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.FileButton_ClickBefore);
            this.modulePath = ((SAPbouiCOM.EditText)(this.GetItem("edArquivo").Specific));
            this.installUpdateModule = ((SAPbouiCOM.Button)(this.GetItem("btInst").Specific));
            this.installUpdateModule.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.InstallUpdateModule_ClickBefore);
            this.moduleGrid = ((SAPbouiCOM.Grid)(this.GetItem("gridArq").Specific));
            this.removeGrid = ((SAPbouiCOM.Grid)(this.GetItem("gridMod").Specific));
            this.removeButtom = ((SAPbouiCOM.Button)(this.GetItem("btModu").Specific));
            this.removeButtom.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.RemoveButtom_ClickBefore);
            this.configTemp = this.UIAPIRawForm.DataSources.DataTables.Item("configTemp");

            // DataTables
            removeDT = removeGrid.DataTable;
            installDT = moduleGrid.DataTable;

            removeGrid.Columns.Item("#").Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
            removeGrid.Columns.Item("#").Width = 25;
            removeGrid.Columns.Item("#").AffectsFormMode = false;

            // click on first tab.
            this.UIAPIRawForm.DataSources.UserDataSources.Item("Folders").Value = "1";
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private void UpdateRemoveGrid()
        {
            configTemp.ExecuteQuery(
                "select 'N' #, U_Name Name, U_Version Version from [@DOVER_MODULES] WHERE U_Type = 'A'");
            removeGrid.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly,
                configTemp.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly));
        }

        private void UpdateInstallGrid()
        {
            configTemp.ExecuteQuery(
                "select U_Name Name, U_Version Version, case when U_Type = 'C' THEN 'Core' else 'AddIn' End Type, case when U_Installed = 'N' then 'Não' else 'Yes' end Installed, '' Status, '...' History from [@DOVER_MODULES]"
                );

            moduleGrid.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly,
                configTemp.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly));
        }

        protected virtual void FileButton_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SelectFileDialog dialog = new SelectFileDialog("C:\\", "",
                Messages.AdminModuleFilterPrefix + "|*.dll;*.exe;*.zip", DialogType.OPEN);
            dialog.Open();
            modulePath.Value = dialog.SelectedFile;
        }

        protected virtual void InstallUpdateModule_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (string.IsNullOrEmpty(modulePath.Value))
            {
                Logger.Error(Messages.AdminEmptyFile);
            }
            else
            {
                string confirmation;
                Logger.Info(Messages.AdminCreatingAppDomain);
                try
                {
                    SAPAppender.SilentMode = true; // Prevent messy log.
                    if (asmLoader.AddInIsValid(modulePath.Value, out confirmation))
                    {
                        if ((!string.IsNullOrEmpty(confirmation)
                            && app.MessageBox(string.Format("{0}\n{1}", Messages.AdminDatabaseChangeWarning, confirmation)) == 1)
                            || string.IsNullOrEmpty(confirmation))
                        {
                            asmLoader.SaveAddIn(modulePath.Value);
                            UpdateInstallGrid();
                            UpdateRemoveGrid();
                            Logger.Info(Messages.AdminSuccessInstall);
                        }
                    }
                }
                finally
                {
                    SAPAppender.SilentMode = false;
                }
            }
        }

        protected virtual void RemoveButtom_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            for (int i = 0; i < removeDT.Rows.Count; i++)
            {
                string val = (string)removeDT.Columns.Item("#").Cells.Item(i).Value;
                string moduleName = (string)removeDT.Columns.Item("Name").Cells.Item(i).Value;

                if (val != null && !string.IsNullOrEmpty(moduleName) && val == "Y")
                {
                    asmLoader.RemoveAddIn(moduleName);
                }
            }
            UpdateInstallGrid();
            UpdateRemoveGrid();
        }
    }
}
