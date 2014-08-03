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
using Castle.Core.Logging;
using SAPbouiCOM;
using Dover.Framework.Attribute;
using Dover.Framework.Factory;
using SAPbouiCOM.Framework;
using Dover.Framework.Form;
using Dover.Framework.Log;

namespace Dover.Framework.Service
{
    public class MenuEventHandler
    {
        public ILogger Logger { get; set; }
        Dictionary<string, List<MenuEventAttribute>> menuEvents = new Dictionary<string, List<MenuEventAttribute>>();

        public MenuEventHandler()
        {
        }

        internal void sapApp_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            List<MenuEventAttribute> events;
            menuEvents.TryGetValue(pVal.MenuUID, out events);

            if (events != null)
            {
                foreach (var e in events)
                {
                    DispatchForm(e, ref pVal, out BubbleEvent);
                }
            }
        }

        private void DispatchForm(MenuEventAttribute menuEvent, ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            // Prevent event from running twice.
            if (!pVal.BeforeAction)
                return;

            try
            {
                var obj = ContainerManager.Container.Resolve(menuEvent.OriginalType);
                if (obj is FormBase || obj is DoverFormBase)
                {
                    Logger.Debug(DebugString.Format(Messages.MenuDispatchInfo, pVal.MenuUID, menuEvent.OriginalType));
                    var method = menuEvent.OriginalType.GetMethod("Show");
                    method.Invoke(obj, null);
                }
                else if (menuEvent.OriginalMethod != null
                    && menuEvent.OriginalMethod.GetGenericArguments().Count() == 0)
                {
                    Logger.Debug(DebugString.Format(Messages.MenuDispatchInfo, pVal.MenuUID, menuEvent.OriginalMethod));
                    menuEvent.OriginalMethod.Invoke(obj, null);
                }
                else
                {
                    Logger.Debug(DebugString.Format(Messages.FileMissing, menuEvent.OriginalType, "?"));
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.MenuDispatchError, pVal.MenuUID, e.Message), e);
            }
        }


        internal void RegisterMenuEvent(MenuEventAttribute menuEventAttribute)
        {
            List<MenuEventAttribute> events;
            Logger.Debug(DebugString.Format(Messages.RegisteringMenuEvent, menuEventAttribute.UniqueUID, menuEventAttribute.OriginalType));
            menuEvents.TryGetValue(menuEventAttribute.UniqueUID, out events);
            if (events == null)
            {
                events = new List<MenuEventAttribute>();
                menuEvents.Add(menuEventAttribute.UniqueUID, events);
            }

            if (!events.Contains(menuEventAttribute))
                events.Add(menuEventAttribute);
        }
    }
}
