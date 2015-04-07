using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using System.Reflection;
using Dover.Framework.Factory;
using Dover.Framework.Attribute;
using Castle.Core.Logging;

namespace Dover.Framework.Service
{
    class AddinAppEventHandler
    {
        private class EventHandlerMap
        {
            internal Type EventHandlerType;
            internal List<Delegate> EventList;
        }

        private ILogger Logger;

        private List<Delegate> formDataEvents = new List<Delegate>();
        private List<Delegate> itemEvents = new List<Delegate>();
        private List<Delegate> printEvents = new List<Delegate>();
        private List<Delegate> progressBarEvents = new List<Delegate>();
        private List<Delegate> reportDataEvents = new List<Delegate>();
        private List<Delegate> rightClickEvents = new List<Delegate>();
        private List<Delegate> serverInvokeEvents = new List<Delegate>();
        private List<Delegate> statusBarEvents = new List<Delegate>();
        private List<Delegate> udoEvents = new List<Delegate>();
        private List<Delegate> widgetEvents = new List<Delegate>();

        public AddinAppEventHandler(ILogger Logger)
        {
            this.Logger = Logger;
        }

        internal void sapApp_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach(var e in formDataEvents)
            {
                ((_IApplicationEvents_FormDataEventEventHandler)e)(ref BusinessObjectInfo, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in itemEvents)
            {
                ((_IApplicationEvents_ItemEventEventHandler)e)(FormUID, ref pVal, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_PrintEvent(ref PrintEventInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in printEvents)
            {
                ((_IApplicationEvents_PrintEventEventHandler)e)(ref eventInfo, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_ProgressBarEvent(ref ProgressBarEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in progressBarEvents)
            {
                ((_IApplicationEvents_ProgressBarEventEventHandler)e)(ref pVal, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_ReportDataEvent(ref ReportDataInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in reportDataEvents)
            {
                ((_IApplicationEvents_ReportDataEventEventHandler)e)(ref eventInfo, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_RightClickEvent(ref ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in rightClickEvents)
            {
                ((_IApplicationEvents_RightClickEventEventHandler)e)(ref eventInfo, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_ServerInvokeCompletedEvent(ref B1iEvent b1iEventArgs, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in serverInvokeEvents)
            {
                ((_IApplicationEvents_ServerInvokeCompletedEventEventHandler)e)(ref b1iEventArgs, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_StatusBarEvent(string Text, BoStatusBarMessageType messageType)
        {
            foreach (var e in statusBarEvents)
            {
                ((_IApplicationEvents_StatusBarEventEventHandler)e)(Text, messageType);
            }
        }

        internal void sapApp_UDOEvent(ref UDOEvent udoEventArgs, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in udoEvents)
            {
                ((_IApplicationEvents_UDOEventEventHandler)e)(ref udoEventArgs, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void sapApp_WidgetEvent(ref WidgetData pWidgetData, out bool BubbleEvent)
        {
            BubbleEvent = true;
            foreach (var e in widgetEvents)
            {
                ((_IApplicationEvents_WidgetEventEventHandler)e)(ref pWidgetData, out BubbleEvent);
                if (!BubbleEvent)
                    break;
            }
        }

        internal void RegisterEvents()
        {
            Assembly currentAsm = AppDomain.CurrentDomain.Load((string)AppDomain.CurrentDomain.GetData("assemblyName"));

            Dictionary<Type, EventHandlerMap> attributeMap = new Dictionary<Type, EventHandlerMap>()
            {
                { typeof(ItemEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_ItemEventEventHandler), EventList = itemEvents} },
                { typeof(FormDataEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_FormDataEventEventHandler), EventList = formDataEvents} },
                { typeof(PrintEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_PrintEventEventHandler), EventList = printEvents} },
                { typeof(ProgressBarEvent), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_ProgressBarEventEventHandler), EventList = progressBarEvents} },
                { typeof(ReportDataEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_ReportDataEventEventHandler), EventList = reportDataEvents} },
                { typeof(RightClickEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_RightClickEventEventHandler), EventList = rightClickEvents} },
                { typeof(ServerInvokeCompletedEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_ServerInvokeCompletedEventEventHandler), EventList = serverInvokeEvents} },
                { typeof(StatusBarEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_StatusBarEventEventHandler), EventList = statusBarEvents} },
                { typeof(UDOEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_UDOEventEventHandler), EventList = udoEvents} },
                { typeof(WidgetEventAttribute), new EventHandlerMap{EventHandlerType = typeof(_IApplicationEvents_WidgetEventEventHandler), EventList = widgetEvents} }
            };
            filterDelegates(attributeMap, currentAsm);
        }

        private void filterDelegates(Dictionary<Type, EventHandlerMap> delegateTypes, Assembly currentAsm)
        {
            var attributes = (from type in currentAsm.GetTypes()
                                  from method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic)
                                  from attribute in method.GetCustomAttributes(true)
                                  where attribute is AppEventAttribute
                                  select new {
                                            Assembly = currentAsm,
                                            Type = type,
                                            Method = method});

            foreach (var a in attributes)
            {
                try
                {
                    var obj = ContainerManager.Container.Resolve(a.Type);
                    var genericDelegate = Delegate.CreateDelegate(delegateTypes[a.Type].EventHandlerType, obj, a.Method);
                    delegateTypes[a.Type].EventList.Add(genericDelegate);
                }
                catch (ArgumentException e)
                {
                    Logger.Error(string.Format(Messages.EventNotRegisteredError, a.Type.Name, a.Method.Name));
                }
            }
        }

    }
}
