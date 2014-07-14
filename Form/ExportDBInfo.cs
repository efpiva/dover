using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using Dover.Framework.Attribute;
using SAPbouiCOM;
using Dover.Framework.DAO;
using SAPbobsCOM;

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

        private BusinessOneDAO b1DAO;

        public ExportDBInfo(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        public override void OnInitializeComponent()
        {
            expDT = this.UIAPIRawForm.DataSources.DataTables.Item("UDO"); // default DT.
            expType = (ComboBox)GetItem("expType").Specific;
            expGrid = (Grid)GetItem("expGrid").Specific;
            exportBT = (Button)GetItem("ExportBT").Specific;
            exportBT.ClickAfter += new _IButtonEvents_ClickAfterEventHandler(exportBT_ClickAfter);
        }

        protected virtual void exportBT_ClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            List<Tuple<object>> codes = new List<Tuple<object>>();
            for (int i = 0; i < expGrid.Rows.SelectedRows.Count; i++)
            {
                codes.Add(
                    new Tuple<object>(
                    expDT.GetValue("Id1", expGrid.Rows.SelectedRows.Item(i, BoOrderType.ot_RowOrder))
                    ));
            }
            string xml = b1DAO.GetXMLBom<UserObjectsMD>(codes.ToArray(), BoObjectTypes.oUserObjectsMD);
        }
    }
}
