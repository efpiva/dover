using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Attribute;
using SAPbouiCOM;
using Dover.Framework.DAO;

namespace Dover.Framework.Form
{
    [Menu(FatherUID = "43523", i18n = "Dover.Framework.Form.Messages.ExportDBInfoMenu", Type = BoMenuType.mt_STRING, UniqueID = "doverExport", ValidateMethod = "IsSuperUser", Position = 1)]
    [Menu(FatherUID = "43523", i18n = "Dover.Framework.Form.Messages.AdminMenu", Type = BoMenuType.mt_STRING, UniqueID = "doverAdmin", ValidateMethod = "IsSuperUser", Position = 1)]
    internal class MenuConfiguration
    {
        private BusinessOneDAO b1DAO;

        public MenuConfiguration(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
        }

        public bool IsSuperUser()
        {
            return b1DAO.IsSuperUser();
        }

    }
}
