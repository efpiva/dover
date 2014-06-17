using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using AddOne.Framework.Form;
using System.Reflection;

namespace AddOne.Framework.Service
{
    public class FormEventHandler
    {
        private Application sapApp;
        private PermissionManager permissionManager;
        private B1SResourceManager resourceManager;

        Dictionary<string, object> formEvents = new Dictionary<string, object>();
        Dictionary<string, AddOneFormBase> events = new Dictionary<string, AddOneFormBase>();
        Dictionary<string, List<AddOneFormBase>> pendingForms = new Dictionary<string, List<AddOneFormBase>>();

        public FormEventHandler(Application sapApp, PermissionManager permissionManager,
            B1SResourceManager resourceManager)
        {
            this.sapApp = sapApp;
            this.permissionManager = permissionManager;
            this.resourceManager = resourceManager;
        }

        internal void RegisterFormLoadBefore(string type, AddOneFormBase form)
        {
            List<AddOneFormBase> pendingList; // forms that does not have UniqueID created by UI.
            if (!pendingForms.TryGetValue(type, out pendingList))
            {
                pendingList = new List<AddOneFormBase>();
                pendingForms.Add(type, pendingList);
            }
            pendingList.Add(form);
        }

        internal void RegisterForms()
        {
            Assembly currentAsm = AppDomain.CurrentDomain.Load((string)AppDomain.CurrentDomain.GetData("assemblyName"));
            var formAttributes = (from type in currentAsm.GetTypes()
                                  from attribute in type.GetCustomAttributes(true)
                                  where attribute is SAPbouiCOM.Framework.FormAttribute
                                  select new { FormAttribute = (SAPbouiCOM.Framework.FormAttribute)attribute, Assembly = currentAsm }).ToList();

            foreach (var asmName in currentAsm.GetReferencedAssemblies())
            {
                Assembly dependency = AppDomain.CurrentDomain.Load(asmName);
                formAttributes.AddRange((from type in dependency.GetTypes()
                                      from attribute in type.GetCustomAttributes(true)
                                      where attribute is SAPbouiCOM.Framework.FormAttribute
                                      select new { FormAttribute = (SAPbouiCOM.Framework.FormAttribute)attribute, Assembly = currentAsm }).ToList() );
            }

            foreach (var attribute in formAttributes)
            {
                RegisterFormEvent(attribute.FormAttribute.FormType);
                resourceManager.ConfigureFormXML(attribute.Assembly.GetName().FullName,
                    attribute.FormAttribute.Resource, attribute.FormAttribute.FormType);
            }            
        }

        private void RegisterFormEvent(string formType)
        {
            if (!formEvents.ContainsKey(formType))
            {
                var eventForm = sapApp.Forms.GetEventForm(formType);
                formEvents.Add(formType, eventForm);
                eventForm.LoadBefore += new _IEventFormEvents_LoadBeforeEventHandler(this.OnFormLoadBefore);
                eventForm.ActivateAfter += new _IEventFormEvents_ActivateAfterEventHandler(this.OnFormActivateAfter);
                eventForm.ActivateBefore += new _IEventFormEvents_ActivateBeforeEventHandler(this.OnFormActivateBefore);
                eventForm.ClickAfter += new _IEventFormEvents_ClickAfterEventHandler(this.OnFormClickAfter);
                eventForm.ClickBefore += new _IEventFormEvents_ClickBeforeEventHandler(this.OnFormClickBefore);
                eventForm.CloseAfter += new _IEventFormEvents_CloseAfterEventHandler(this.OnFormCloseAfter);
                eventForm.CloseBefore += new _IEventFormEvents_CloseBeforeEventHandler(this.OnFormCloseBefore);
                eventForm.DataAddAfter += new _IEventFormEvents_DataAddAfterEventHandler(this.OnFormDataAddAfter);
                eventForm.DataAddBefore += new _IEventFormEvents_DataAddBeforeEventHandler(this.OnFormDataAddBefore);
                eventForm.DataDeleteAfter += new _IEventFormEvents_DataDeleteAfterEventHandler(this.OnFormDataDeleteAfter);
                eventForm.DataDeleteBefore += new _IEventFormEvents_DataDeleteBeforeEventHandler(this.OnFormDataDeleteBefore);
                eventForm.DataLoadAfter += new _IEventFormEvents_DataLoadAfterEventHandler(this.OnFormDataLoadAfter);
                eventForm.DataLoadBefore += new _IEventFormEvents_DataLoadBeforeEventHandler(this.OnFormDataLoadBefore);
                eventForm.DataUpdateAfter += new _IEventFormEvents_DataUpdateAfterEventHandler(this.OnFormDataUpdateAfter);
                eventForm.DataUpdateBefore += new _IEventFormEvents_DataUpdateBeforeEventHandler(this.OnFormDataUpdateBefore);
                eventForm.DeactivateAfter += new _IEventFormEvents_DeactivateAfterEventHandler(this.OnFormDeactivateAfter);
                eventForm.DeactivateBefore += new _IEventFormEvents_DeactivateBeforeEventHandler(this.OnFormDeactivateBefore);
                eventForm.KeyDownAfter += new _IEventFormEvents_KeyDownAfterEventHandler(this.OnFormKeyDownAfter);
                eventForm.KeyDownBefore += new _IEventFormEvents_KeyDownBeforeEventHandler(this.OnFormKeyDownBefore);
                eventForm.LayoutKeyAfter += new _IEventFormEvents_LayoutKeyAfterEventHandler(this.OnFormLayoutKeyAfter);
                eventForm.LayoutKeyBefore += new _IEventFormEvents_LayoutKeyBeforeEventHandler(this.OnFormLayoutKeyBefore);
                eventForm.LoadAfter += new _IEventFormEvents_LoadAfterEventHandler(this.OnFormLoadAfter);
                eventForm.MenuHighlightAfter += new _IEventFormEvents_MenuHighlightAfterEventHandler(this.OnFormMenuHighlightAfter);
                eventForm.MenuHighlightBefore += new _IEventFormEvents_MenuHighlightBeforeEventHandler(this.OnFormMenuHighlightBefore);
                eventForm.PrintAfter += new _IEventFormEvents_PrintAfterEventHandler(this.OnFormPrintAfter);
                eventForm.PrintBefore += new _IEventFormEvents_PrintBeforeEventHandler(this.OnFormPrintBefore);
                eventForm.ReportDataAfter += new _IEventFormEvents_ReportDataAfterEventHandler(this.OnFormReportDataAfter);
                eventForm.ReportDataBefore += new _IEventFormEvents_ReportDataBeforeEventHandler(this.OnFormReportDataBefore);
                eventForm.ResizeAfter += new _IEventFormEvents_ResizeAfterEventHandler(this.OnFormResizeAfter);
                eventForm.ResizeBefore += new _IEventFormEvents_ResizeBeforeEventHandler(this.OnFormResizeBefore);
                eventForm.RightClickAfter += new _IEventFormEvents_RightClickAfterEventHandler(this.OnFormRightClickAfter);
                eventForm.RightClickBefore += new _IEventFormEvents_RightClickBeforeEventHandler(this.OnFormRightClickBefore);
                eventForm.UnloadAfter += new _IEventFormEvents_UnloadAfterEventHandler(this.OnFormUnloadAfter);
                eventForm.UnloadBefore += new _IEventFormEvents_UnloadBeforeEventHandler(this.OnFormUnloadBefore);
                eventForm.VisibleAfter += new _IEventFormEvents_VisibleAfterEventHandler(this.OnFormVisibleAfter);
            }
            else
            {
                throw new ArgumentException(string.Format(Messages.FormTypeNotUnique, formType));
            }
        }

        /*
         * Register form so we can get FormUID when form is being created.
         */
        internal void RegisterForm(string uniqueID, AddOneFormBase form)
        {
            if (!events.ContainsKey(uniqueID)) // prevent duplicates. Shouldn't happen.
            {
                events.Add(uniqueID, form);
                form.OnInitializeFormEvents();
            }
        }

        private void OnFormLoadBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            List<AddOneFormBase> pendingList; // forms that does not have UniqueID.
            IForm form = sapApp.Forms.Item(pVal.FormUID);
            string key = form.TypeEx;
            BubbleEvent = true;

            if (pendingForms.TryGetValue(key, out pendingList))
            {
                pendingForms.Remove(key);
                foreach(var addOneForm in pendingList)
                {
                    addOneForm.OnFormLoadBefore(pVal, out BubbleEvent);
                }
            }
        }

        private void OnFormLoadAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormLoadAfter(pVal);
                events.Remove(pVal.FormUID);
            }
        }

        private void OnFormUnloadBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormUnloadBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormUnloadAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormUnloadAfter(pVal);
            }
        }

        private void OnFormActivateBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormActivateBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormActivateAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormActivateAfter(pVal);
            }
        }

        private void OnFormDeactivateBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDeactivateBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormDeactivateAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDeactivateAfter(pVal);
            }
        }

        private void OnFormResizeBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormResizeBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormResizeAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormResizeAfter(pVal);
            }
        }

        private void OnFormMenuHighlightBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormMenuHighlightBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormMenuHighlightAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormMenuHighlightAfter(pVal);
            }
        }

        private void OnFormDataAddBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataAddBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormDataAddAfter(ref BusinessObjectInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataAddAfter(ref pVal);
            }
        }

        private void OnFormDataUpdateBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataUpdateBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormDataUpdateAfter(ref BusinessObjectInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataUpdateAfter(ref pVal);
            }
        }

        private void OnFormDataDeleteBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataDeleteBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormDataDeleteAfter(ref BusinessObjectInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataDeleteAfter(ref pVal);
            }
        }

        private void OnFormDataLoadBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataLoadBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormDataLoadAfter(ref BusinessObjectInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormDataLoadAfter(ref pVal);
            }
        }

        private void OnFormKeyDownBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormKeyDownBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormKeyDownAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormKeyDownAfter(pVal);
            }
        }

        private void OnFormLayoutKeyBefore(ref LayoutKeyInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormLayoutKeyBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormLayoutKeyAfter(ref LayoutKeyInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormLayoutKeyAfter(ref pVal);
            }
        }

        private void OnFormPrintBefore(ref PrintEventInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormPrintBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormPrintAfter(ref PrintEventInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormPrintAfter(ref pVal);
            }
        }

        private void OnFormReportDataBefore(ref PrintEventInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormReportDataBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormReportDataAfter(ref PrintEventInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormReportDataAfter(ref pVal);
            }
        }

        private void OnFormClickBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormClickBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormClickAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormClickAfter(pVal);
            }
        }

        private void OnFormRightClickBefore(ref ContextMenuInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormRightClickBefore(ref pVal, out BubbleEvent);
            }
        }

        private void OnFormRightClickAfter(ref ContextMenuInfo pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormRightClickAfter(ref pVal);
            }
        }

        private void OnFormCloseBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormCloseBefore(pVal, out BubbleEvent);
            }
        }

        private void OnFormCloseAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormCloseAfter(pVal);
            }
        }

        private void OnFormVisibleAfter(SBOItemEventArg pVal)
        {
            AddOneFormBase addOneForm;
            if (events.TryGetValue(pVal.FormUID, out addOneForm))
            {
                addOneForm.OnFormVisibleAfter(pVal);
            }
        }

    }

}
