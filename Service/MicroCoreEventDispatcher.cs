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
using SAPbouiCOM;
using Dover.Framework.Attribute;
using Dover.Framework.Factory;
using Castle.Core.Logging;
using System.ServiceModel;

namespace Dover.Framework.Service
{
    /// <summary>
    /// User by MicroCore. Just AppEvent (reboot / reload addins).
    /// </summary>
    public class MicroCoreEventDispatcher
    {
        public ILogger Logger { get; set; }
        private SAPbouiCOM.Application sapApp;
        private AppEventHandler appEventHandler;

        public MicroCoreEventDispatcher(SAPbouiCOM.Application sapApp, AppEventHandler appEventHandler)
        {
            this.sapApp = sapApp;
            this.appEventHandler = appEventHandler;
        }

        internal void RegisterEvents()
        {
            sapApp.AppEvent += new _IApplicationEvents_AppEventEventHandler(appEventHandler.sapApp_AppEvent);
        }
    }
}
