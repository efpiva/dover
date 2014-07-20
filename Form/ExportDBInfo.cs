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
using SAPbouiCOM;
using Dover.Framework.DAO;
using SAPbobsCOM;
using System.IO;
using Castle.Core.Logging;

namespace Dover.Framework.Form
{

    [MenuEvent(UniqueUID = "doverExport")]
    [FormAttribute("dover.exportDBInfo", "Dover.Framework.Form.ExportDBInfo.srf")]
    public class ExportDBInfo : DoverUserFormBase
    {
        private Button exportBT;
        private Grid expGrid;
        private DataTable expDT;
        private ComboBox expType;

        public ILogger Logger { get; set; }

        private BusinessOneDAO b1DAO;

        public ExportDBInfo(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        public override void OnInitializeComponent()
        {
            expDT = this.UIAPIRawForm.DataSources.DataTables.Item("Grid"); 

            expType = (ComboBox)GetItem("expType").Specific;
            expGrid = (Grid)GetItem("expGrid").Specific;
            exportBT = (Button)GetItem("ExportBT").Specific;
            exportBT.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(exportBT_ClickAfter);
            expType.ComboSelectAfter += new _IComboBoxEvents_ComboSelectAfterEventHandler(expType_ComboSelectAfter);

            expType.Select("UDO");
        }

        protected virtual void expType_ComboSelectAfter(object sboObject, SBOItemEventArg pVal)
        {
            DataTable dt = this.UIAPIRawForm.DataSources.DataTables.Item(expType.Value);
            expDT.LoadSerializedXML(BoDataTableXmlSelect.dxs_DataOnly, dt.SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly));
        }

        protected virtual void exportBT_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            object[] codes = GetDataTableCodes();
            string xml = string.Empty;
            switch (expType.Value)
            {
                case "UDO":
                    xml = b1DAO.GetXMLBom<UserObjectsMD>(codes, BoObjectTypes.oUserObjectsMD);
                    break;
                case "UDT":
                    xml = b1DAO.GetXMLBom<UserTablesMD>(codes, BoObjectTypes.oUserTables);
                    break;
                case "UDF":
                    xml = b1DAO.GetXMLBom<UserFieldsMD>(codes, BoObjectTypes.oUserFields);
                    break;
            }

            SelectFileDialog dialog = new SelectFileDialog("C:\\", "",
                "|*.xml", DialogType.SAVE);
            dialog.Open();
            if (!string.IsNullOrEmpty(dialog.SelectedFile))
            {
                File.WriteAllText(dialog.SelectedFile, xml);
                Logger.Info(Messages.ExportDBInfoSuccess);
                this.UIAPIRawForm.Close();
            }
            else
            {
                Logger.Error(Messages.ExportDBInfoFileNotFound);
            }
        }

        private object[] GetDataTableCodes()
        {
            List<Tuple<object>> tuple1 = new List<Tuple<object>>();
            List<Tuple<object, object>> tuple2 = new List<Tuple<object, object>>();

            new List<Tuple<object>>();
            for (int i = 0; i < expGrid.Rows.SelectedRows.Count; i++)
            {
                tuple1.Add(
                    new Tuple<object>(
                        expDT.GetValue("Id1", expGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder))
                    ));
                tuple2.Add(
                    new Tuple<object, object>(
                        expDT.GetValue("Id1", expGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder)),
                        expDT.GetValue("Id2", expGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder))
                    ));
            }
            switch (expType.Value)
            {
                case "UDO":
                case "UDT":
                    return tuple1.ToArray();
            }
            return tuple2.ToArray();
        }
    }
}
