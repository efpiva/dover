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
using Dover.Framework.Form;
using SAPbouiCOM;
using Dover.Framework.Service;
using Dover.Framework.DAO;

namespace Dover.Framework.Form
{

    [MenuEvent(UniqueUID = "doverMngmnt")]
    [FormAttribute("dover.StartupManagement", "Dover.Framework.Form.AddInStartupManagement.srf")]
    internal class AddInStartupManagement : DoverUserFormBase
    {
        private SAPbouiCOM.Grid gridUser;
        private SAPbouiCOM.Grid gridCfg;
        private SAPbouiCOM.Grid generalGrid;
        private DataTable configTemp;
        public PermissionManager PermissionManager { get; set; }

        private string userConfigSQLTemplate;

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            userConfigSQLTemplate = this.GetSQL("addInStartupManagementSQLTemplate.sql");
            this.gridUser = ((SAPbouiCOM.Grid)(this.GetItem("gridUser").Specific));
            this.gridCfg = ((SAPbouiCOM.Grid)(this.GetItem("gridCfg").Specific));
            this.generalGrid = ((SAPbouiCOM.Grid)(this.GetItem("gridGnrl").Specific));
            this.configTemp = this.UIAPIRawForm.DataSources.DataTables.Item("configTemp");
            this.OnCustomInitialize();
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }


        private void OnCustomInitialize()
        {
            for (int i = 0; i < generalGrid.Columns.Count; i++)
            {
                var activeCol = generalGrid.Columns.Item(i).UniqueID == "Status";

                if (activeCol)
                {
                    generalGrid.Columns.Item(i).ComboSelectBefore += new _IColumnEvents_ComboSelectBeforeEventHandler(StatusChangeBefore);
                    generalGrid.Columns.Item(i).ComboSelectAfter += new _IColumnEvents_ComboSelectAfterEventHandler(GeneralGridStatusChange);
                    ((ComboBoxColumn)generalGrid.Columns.Item(i)).ValidValues.Add("A", Messages.MngmntActive);
                    ((ComboBoxColumn)generalGrid.Columns.Item(i)).ValidValues.Add("I", Messages.MngmntInactive);
                    ((ComboBoxColumn)generalGrid.Columns.Item(i)).DisplayType = BoComboDisplayType.cdt_Description;
                }
            }

            // user
            for (int i = 0; i < gridUser.DataTable.Rows.Count; i++)
            {
                if (gridUser.DataTable.Columns.Item("UserName").Cells.Item(i).Value.ToString() == "manager")
                    gridUser.Rows.SelectedRows.Add(i);
            }

            gridUser.PressedAfter += new _IGridEvents_PressedAfterEventHandler(UpdateUserConfiguration);

            // config
            gridCfg.ComboSelectBefore += new _IGridEvents_ComboSelectBeforeEventHandler(StatusChangeBefore);
            gridCfg.ComboSelectAfter += new _IGridEvents_ComboSelectAfterEventHandler(UserConfigStatusChange);

            ((ComboBoxColumn)gridCfg.Columns.Item("Status")).ValidValues.Add("A", Messages.MngmntActive);
            ((ComboBoxColumn)gridCfg.Columns.Item("Status")).ValidValues.Add("I", Messages.MngmntInactive);
            ((ComboBoxColumn)gridCfg.Columns.Item("Status")).ValidValues.Add("D", Messages.MngmntDefault);
            ((ComboBoxColumn)gridCfg.Columns.Item("Status")).DisplayType = BoComboDisplayType.cdt_Description;
            ((ComboBoxColumn)gridCfg.Columns.Item("Status")).AffectsFormMode = false;

            // click on first tab
            this.UIAPIRawForm.DataSources.UserDataSources.Item("Folders").Value = "1";
        }

        protected virtual void UpdateUserConfiguration(object data, SBOItemEventArg args)
        {
            if (gridUser.Rows.SelectedRows.Count > 0)
            {
                var index = gridUser.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                var username = gridUser.DataTable.Columns.Item("UserName").Cells.Item(index).Value.ToString();
                configTemp.ExecuteQuery(string.Format(userConfigSQLTemplate, username));

                if (!(configTemp.Rows.Count == 1 && string.IsNullOrEmpty(configTemp.GetValue("Code", 0).ToString())))
                {
                    gridCfg.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly,
                        configTemp.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly));

                    gridCfg.ComboSelectBefore += new _IGridEvents_ComboSelectBeforeEventHandler(StatusChangeBefore);
                    gridCfg.ComboSelectAfter += new _IGridEvents_ComboSelectAfterEventHandler(UserConfigStatusChange);
                }
            }
        }

        protected virtual void GeneralGridStatusChange(object data, SBOItemEventArg args)
        {
            if (args.ColUID == "Status")
            {
                var selectedRow = args.Row;
                var code = generalGrid.DataTable.Columns.Item("Code").Cells.Item(selectedRow).Value.ToString();
                var status = generalGrid.DataTable.Columns.Item("Status").Cells.Item(selectedRow).Value.ToString();

                Permission permission = PermissionManager.ParsePermissionStr(status);
                PermissionManager.ConfigureAddIn(code, permission);

                if (this.UIAPIRawForm.Mode == BoFormMode.fm_UPDATE_MODE)
                    this.UIAPIRawForm.Mode = BoFormMode.fm_OK_MODE;

                this.UIAPIRawForm.Freeze(false);
            }
        }

        // work arround because AffectsFormMode for ComboSelect is not working the way we expect.
        protected virtual void StatusChangeBefore(object data, SBOItemEventArg args, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (args.ColUID == "Status")
            {
                this.UIAPIRawForm.Freeze(true);
            }
        }

        protected virtual void UserConfigStatusChange(object data, SBOItemEventArg args)
        {
            if (args.ColUID == "Status" && gridUser.Rows.SelectedRows.Count > 0)
            {
                var index = gridUser.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                var username = gridUser.DataTable.Columns.Item("UserName").Cells.Item(index).Value.ToString();
                var selectedRow = args.Row;
                var code = gridCfg.DataTable.Columns.Item("Code").Cells.Item(selectedRow).Value.ToString();
                var status = gridCfg.DataTable.Columns.Item("Status").Cells.Item(selectedRow).Value.ToString();

                Permission permission = PermissionManager.ParsePermissionStr(status);
                PermissionManager.ConfigureAddIn(code, username, permission);

                if (this.UIAPIRawForm.Mode == BoFormMode.fm_UPDATE_MODE)
                    this.UIAPIRawForm.Mode = BoFormMode.fm_OK_MODE;

                this.UIAPIRawForm.Freeze(false);
            }
        }
    }
}
