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
using Dover.Framework.Attribute;
using Dover.Framework.Service;
using Castle.Core.Logging;
using SAPbouiCOM;
using Dover.Framework.Form;
using Dover.Framework.Log;
using Dover.Framework.DAO;
using Dover.Framework.Interface;
using Dover.Framework.Factory;

namespace Dover.Framework.Form
{

    [MenuEvent(UniqueUID = "doverAdmin")]
    [FormAttribute("dover.formAdmin", "Dover.Framework.Form.Admin.srf")]
    internal class Admin : DoverUserFormBase
    {
        private DataTable moduleDT;
        private DataTable configTemp;

        private AssemblyManager asmLoader;
        private IAppEventHandler appEventHandler;
        private IAddinManager frameworkAddinManager;
        private ILogger Logger;

        public Admin()
        {
            this.asmLoader = ContainerManager.Container.Resolve<AssemblyManager>();
            this.appEventHandler = ContainerManager.Container.Resolve<IAppEventHandler>();
            this.frameworkAddinManager = ContainerManager.Container.Resolve<IAddinManager>();
            this.Logger = ContainerManager.Container.Resolve<ILogger>();
            UpdateAddinStatus();
        }


        // UI components
        private SAPbouiCOM.EditText modulePath;
        private SAPbouiCOM.Button installUpdateModule;
        private SAPbouiCOM.Button fileSelector;
        private SAPbouiCOM.Grid moduleGrid;
        private SAPbouiCOM.Button removeButton;
        private SAPbouiCOM.Button startButton;
        private SAPbouiCOM.Button shutdownButton;
        private SAPbouiCOM.Button installButton;

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
            this.removeButton = ((SAPbouiCOM.Button)(this.GetItem("btModu").Specific));
            this.removeButton.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.RemoveButtom_ClickAfter);
            this.startButton = ((SAPbouiCOM.Button)(this.GetItem("btStart").Specific));
            this.startButton.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(startButton_ClickAfter);
            this.shutdownButton = ((SAPbouiCOM.Button)(this.GetItem("btStop").Specific));
            this.shutdownButton.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(shutdownButton_ClickAfter);
            this.installButton = ((SAPbouiCOM.Button)(this.GetItem("btInstall").Specific));
            this.installButton.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(installButton_ClickAfter);
            this.configTemp = this.UIAPIRawForm.DataSources.DataTables.Item("configTemp");
            this.moduleGrid.ClickAfter += new _IGridEvents_ClickAfterEventHandler(moduleGrid_ClickAfter);

            ((ComboBoxColumn)moduleGrid.Columns.Item("Installed")).ValidValues.Add("Y", Messages.Yes);
            ((ComboBoxColumn)moduleGrid.Columns.Item("Installed")).ValidValues.Add("N", Messages.No);
            ((ComboBoxColumn)moduleGrid.Columns.Item("Installed")).DisplayType = BoComboDisplayType.cdt_Description;

            ((ComboBoxColumn)moduleGrid.Columns.Item("Status")).ValidValues.Add("R", Messages.AdminRunning);
            ((ComboBoxColumn)moduleGrid.Columns.Item("Status")).ValidValues.Add("S", Messages.AdminStopped);
            ((ComboBoxColumn)moduleGrid.Columns.Item("Status")).DisplayType = BoComboDisplayType.cdt_Description;

            moduleDT = moduleGrid.DataTable;

            // click on first tab.
            this.UIAPIRawForm.DataSources.UserDataSources.Item("Folders").Value = "1";
        }

        protected virtual void installButton_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            for (int i = 0; i < moduleGrid.Rows.SelectedRows.Count; i++)
            {
                int rowId = moduleGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder);
                string moduleName = (string)moduleDT.GetValue("Name", rowId);
                string type = (string)moduleDT.GetValue("Type", rowId);
                if (type == "AddIn")
                    frameworkAddinManager.InstallAddin(moduleName);
            }
            UpdateInstallGrid();
            UpdateLicenseGrid();
            moduleGrid_ClickAfter(sboObject, pVal);
        }

        protected virtual void shutdownButton_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            for (int i = 0; i < moduleGrid.Rows.SelectedRows.Count; i++)
            {
                int rowId = moduleGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder);
                string moduleName = (string)moduleDT.GetValue("Name", rowId);
                string type = (string)moduleDT.GetValue("Type", rowId);
                if (type == "AddIn")
                    frameworkAddinManager.ShutdownAddin(moduleName);
            }
            UpdateInstallGrid();
            UpdateLicenseGrid();
            moduleGrid_ClickAfter(sboObject, pVal);
        }

        protected virtual void startButton_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            for (int i = 0; i < moduleGrid.Rows.SelectedRows.Count; i++)
            {
                int rowId = moduleGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder);
                string moduleName = (string)moduleDT.GetValue("Name", rowId);
                string type = (string)moduleDT.GetValue("Type", rowId);
                if (type == "AddIn")
                    frameworkAddinManager.StartAddin(moduleName);
            }
            UpdateInstallGrid();
            UpdateLicenseGrid();
            moduleGrid_ClickAfter(sboObject, pVal);
        }

        private void UpdateAddinStatus()
        {
            for (int i = 0; i < moduleDT.Rows.Count; i++)
            {
                string name = (string)moduleDT.GetValue("Name", i);
                string type = (string)moduleDT.GetValue("Type", i);
                string status;
                if (type == "AddIn")
                {
                    AddinStatus addinStatus = frameworkAddinManager.GetAddinStatus(name);
                    status = (addinStatus == AddinStatus.Running) ? "R" : "S";
                }
                else
                {
                    status = "R"; // mark framework as running.
                }
                moduleDT.SetValue("Status", i, status);
            }
        }

        protected virtual void moduleGrid_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                bool enableRemove = false, enableStart = false, enableStop = false, enableInstall = false, isAddin;

                for (int i = 0; i < moduleGrid.Rows.SelectedRows.Count; i++)
                {
                    string type = (string)moduleDT.GetValue("Type", moduleGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_SelectionOrder));
                    isAddin = (type == "AddIn");
                    if (isAddin)
                        enableRemove = true;
                    string status = (string)moduleDT.GetValue("Status", moduleGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_SelectionOrder));
                    string installed = (string)moduleDT.GetValue("Installed", moduleGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_SelectionOrder));
                    enableInstall = (installed == "N") || enableInstall;
                    enableStart = ((status == "S") && isAddin) || enableStart;
                    enableStop = (!enableStart && isAddin) || enableStop;
                }

                this.removeButton.Item.Enabled = enableRemove;
                this.startButton.Item.Enabled = enableStart;
                this.shutdownButton.Item.Enabled = enableStop;
                this.installButton.Item.Enabled = enableInstall;

                // History column event.
                if (pVal.ColUID == "History")
                {
                    string module = (string)moduleDT.GetValue("Name", pVal.Row);
                    var logForm = CreateForm<ChangeLog>();
                    logForm.LogMessage.Value = frameworkAddinManager.GetAddinChangeLog(module);
                    logForm.Show();
                }
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        protected virtual void UpdateLicenseGrid()
        {
            // overrid on AddInSetup.
        }

        protected void UpdateInstallGrid()
        {
            try
            {
                this.UIAPIRawForm.Freeze(true);
                configTemp.ExecuteQuery(this.GetSQL("adminModDT.sql"));

                moduleGrid.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly,
                    configTemp.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly));
                UpdateAddinStatus();
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        protected virtual void FileButton_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SelectFileDialog dialog = new SelectFileDialog("C:\\", "",
                Messages.AdminModuleFilterPrefix + "|*.dover", DialogType.OPEN);
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
                        if (string.IsNullOrEmpty(confirmation))
                        {
                            InstallAddin();
                        }
                        else
                        {
                            GetUserConfirmation(confirmation);
                        }
                    }
                    else
                    {
                        SAPAppender.SilentMode = false;
                        Logger.Error(Messages.AdminInvalidAddin);
                    }
                }
                finally
                {
                    SAPAppender.SilentMode = false;
                }
            }
        }

        private void GetUserConfirmation(string xml)
        {
            DBChange dbChangeForm = CreateForm<DBChange>();
            dbChangeForm.BaseForm = this;
            dbChangeForm.DBChangeDT.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly, xml);
            dbChangeForm.Show();
        }

        internal void InstallAddin()
        {
            string addinName = asmLoader.SaveAddIn(modulePath.Value);
            if (addinName == "Framework")
            {
                if (app.MessageBox(Messages.AdminConfirmReboot, 1, Messages.AdminOK, Messages.AdminCancel) == 1)
                {
                    appEventHandler.Reboot();
                }
            }
            else if (!string.IsNullOrEmpty(addinName))
            {
                if (frameworkAddinManager.GetAddinStatus(addinName) == AddinStatus.Running)
                {
                    frameworkAddinManager.ShutdownAddin(addinName);
                    frameworkAddinManager.StartAddin(addinName);
                }
                UpdateInstallGrid();
                UpdateLicenseGrid();
                SAPAppender.SilentMode = false;
                Logger.Info(Messages.AdminSuccessInstall);
            }
            
        }

        protected virtual void RemoveButtom_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            for (int i = 0; i < moduleGrid.Rows.SelectedRows.Count; i++)
            {
                int rowId = moduleGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder);
                string moduleName = (string)moduleDT.GetValue("Name", rowId);
                string type = (string)moduleDT.GetValue("Type", rowId);
                if (type == "AddIn")
                {
                    frameworkAddinManager.ShutdownAddin(moduleName);
                    asmLoader.RemoveAddIn(moduleName);
                }
            }
            UpdateInstallGrid();
            UpdateLicenseGrid();
            moduleGrid_ClickAfter(sboObject, pVal);
        }
    }
}
