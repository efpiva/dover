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
using Dover.Framework.Attribute;
using SAPbouiCOM;
using Dover.Framework.DAO;
using Dover.Framework.Service;

namespace Dover.Framework.Form
{
    [Menu(FatherUID = "43523", i18n = "Dover.Framework.Form.Messages.AdminMenu", Type = BoMenuType.mt_STRING, UniqueID = "doverAdmin", ValidateMethod = "IsSuperUser", Position = 3)]
    [Menu(FatherUID = "43523", i18n = "Dover.Framework.Form.Messages.MngmntMenu", Type = BoMenuType.mt_STRING, UniqueID = "doverMngmnt", ValidateMethod = "IsSuperUser", Position = 4)]
    [Menu(FatherUID = "43523", i18n = "Dover.Framework.Form.Messages.ExportDBInfoMenu", Type = BoMenuType.mt_STRING, UniqueID = "doverExport", ValidateMethod = "IsSuperUser", Position = 5)]
    [Menu(FatherUID = "43523", i18n = "Dover.Framework.Form.Messages.ShutdownMenu", Type = BoMenuType.mt_STRING, UniqueID = "doverShutdown", ValidateMethod = "IsDebug", Position = 6)]
    internal class MenuConfiguration
    {
        private BusinessOneDAO b1DAO;
        private AppEventHandler appEvent;

        public MenuConfiguration(BusinessOneDAO b1DAO, AppEventHandler appEvent)
        {
            this.b1DAO = b1DAO;
            this.appEvent = appEvent;
        }

        [MenuEvent(UniqueUID="doverShutdown")]
        public void ShutdownEvent()
        {
            appEvent.ShutDown();
        }

        public bool IsSuperUser()
        {
            return b1DAO.IsSuperUser();
        }

        public bool IsDebug()
        {
            return System.Diagnostics.Debugger.IsAttached;
        }

    }
}
