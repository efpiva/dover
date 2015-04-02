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
using Dover.Framework.Form;
using Dover.Framework.Interface;

namespace Dover.Framework.Service
{
    /// <summary>
    /// User just by Inception / addins. No App event.
    /// </summary>
    internal class EventDispatcher : MarshalByRefObject, IEventDispatcher
    {
        public ILogger Logger { get; set; }
        private SAPbouiCOM.Application sapApp;
        private MenuEventHandler menuHandler;
        private AddinAppEventHandler addinAppEventHandler;

        public EventDispatcher(SAPbouiCOM.Application sapApp, MenuEventHandler menuHandler,
            AddinAppEventHandler addinAppEventHandler)
        {
            this.sapApp = sapApp;
            this.menuHandler = menuHandler;
            this.addinAppEventHandler = addinAppEventHandler;
        }

        void IEventDispatcher.RegisterEvents()
        {
            sapApp.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(menuHandler.sapApp_MenuEvent);
            sapApp.FormDataEvent += new _IApplicationEvents_FormDataEventEventHandler(addinAppEventHandler.sapApp_FormDataEvent);
            sapApp.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(addinAppEventHandler.sapApp_ItemEvent);
            sapApp.PrintEvent += new _IApplicationEvents_PrintEventEventHandler(addinAppEventHandler.sapApp_PrintEvent);
            sapApp.ProgressBarEvent += new _IApplicationEvents_ProgressBarEventEventHandler(addinAppEventHandler.sapApp_ProgressBarEvent);
            sapApp.ReportDataEvent += new _IApplicationEvents_ReportDataEventEventHandler(addinAppEventHandler.sapApp_ReportDataEvent);
            sapApp.RightClickEvent += new _IApplicationEvents_RightClickEventEventHandler(addinAppEventHandler.sapApp_RightClickEvent);
            sapApp.ServerInvokeCompletedEvent += new _IApplicationEvents_ServerInvokeCompletedEventEventHandler(addinAppEventHandler.sapApp_ServerInvokeCompletedEvent);
            sapApp.StatusBarEvent += new _IApplicationEvents_StatusBarEventEventHandler(addinAppEventHandler.sapApp_StatusBarEvent);
            sapApp.UDOEvent += new _IApplicationEvents_UDOEventEventHandler(addinAppEventHandler.sapApp_UDOEvent);
            sapApp.WidgetEvent += new _IApplicationEvents_WidgetEventEventHandler(addinAppEventHandler.sapApp_WidgetEvent);
        }

        void IEventDispatcher.UnregisterEvents()
        {
            sapApp.MenuEvent -= new _IApplicationEvents_MenuEventEventHandler(menuHandler.sapApp_MenuEvent);
            sapApp.FormDataEvent -= new _IApplicationEvents_FormDataEventEventHandler(addinAppEventHandler.sapApp_FormDataEvent);
            sapApp.ItemEvent -= new _IApplicationEvents_ItemEventEventHandler(addinAppEventHandler.sapApp_ItemEvent);
            sapApp.PrintEvent -= new _IApplicationEvents_PrintEventEventHandler(addinAppEventHandler.sapApp_PrintEvent);
            sapApp.ProgressBarEvent -= new _IApplicationEvents_ProgressBarEventEventHandler(addinAppEventHandler.sapApp_ProgressBarEvent);
            sapApp.ReportDataEvent -= new _IApplicationEvents_ReportDataEventEventHandler(addinAppEventHandler.sapApp_ReportDataEvent);
            sapApp.RightClickEvent -= new _IApplicationEvents_RightClickEventEventHandler(addinAppEventHandler.sapApp_RightClickEvent);
            sapApp.ServerInvokeCompletedEvent -= new _IApplicationEvents_ServerInvokeCompletedEventEventHandler(addinAppEventHandler.sapApp_ServerInvokeCompletedEvent);
            sapApp.StatusBarEvent -= new _IApplicationEvents_StatusBarEventEventHandler(addinAppEventHandler.sapApp_StatusBarEvent);
            sapApp.UDOEvent -= new _IApplicationEvents_UDOEventEventHandler(addinAppEventHandler.sapApp_UDOEvent);
            sapApp.WidgetEvent -= new _IApplicationEvents_WidgetEventEventHandler(addinAppEventHandler.sapApp_WidgetEvent);
        }
    }
}
