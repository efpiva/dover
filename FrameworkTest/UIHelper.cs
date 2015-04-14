using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameworkTest
{
    internal static class UIHelper
    {
        internal static SAPbouiCOM.Form GetFormAfterAction(string formType, SAPbouiCOM.Application application, Action invoke)
        {
            int beforeCount = GetFormTypeCount(formType, application);
            invoke();
            int afterCount = GetFormTypeCount(formType, application);
            Assert.AreNotSame(beforeCount, afterCount);
            return application.Forms.GetForm(formType, beforeCount);
        }

        internal static int GetFormTypeCount(string formType, SAPbouiCOM.Application application)
        {
            int count = 1;
            for (int i = 0; i < application.Forms.Count; i++)
            {
                if (application.Forms.Item(i).TypeEx == formType)
                    count++;
            }
            return count;
        }

        internal static string ExportDTXML(SAPbouiCOM.Form form, string dtName)
        {
            SAPbouiCOM.DataTable dt = form.DataSources.DataTables.Item(dtName);
            Assert.IsNotNull(dt);
            return dt.SerializeAsXML(SAPbouiCOM.BoDataTableXmlSelect.dxs_DataOnly);
        }
    }
}
