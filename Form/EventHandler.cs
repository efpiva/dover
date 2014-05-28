using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.Core.Logging;
using SAPbouiCOM.Framework;
using SAPbouiCOM;
using AddOne.Framework.Factory;

namespace AddOne.Framework.Form
{
    public static class EventHandler
    {

        private static ILogger Logger = ContainerManager.Container.Resolve<ILogger>();

        public static _IColumnEvents_ComboSelectBeforeEventHandler ExceptionHandler(this _IColumnEvents_ComboSelectBeforeEventHandler eventTrigger, FormBase form)
        {
            _IColumnEvents_ComboSelectBeforeEventHandler retFunction = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) =>
            {
                BubbleEvent = true;
                try
                {
                    eventTrigger(sboObject, pVal, out BubbleEvent);
                }
                catch (Exception e)
                {
                    if (form != null && form.UIAPIRawForm != null)
                        form.UIAPIRawForm.Freeze(false); // force unfreeze in case of error.

                    Assembly addinAssembly = eventTrigger.Method.DeclaringType.Assembly;
                    Version objVersion = addinAssembly.GetName().Version;
                    String addInName = addinAssembly.GetName().Name;
                    String addInVersion = objVersion.Major.ToString() + "." + objVersion.Minor.ToString() + "." + objVersion.Build.ToString()
                                + "." + objVersion.Revision;

                    Logger.Error(String.Format(Messages.AddInError, addInName, addInVersion), e);
                }
            };
            return retFunction;
        }

        public static _IGridEvents_PressedAfterEventHandler ExceptionHandler(this _IGridEvents_PressedAfterEventHandler eventTrigger, FormBase form)
        {
            _IGridEvents_PressedAfterEventHandler retFunction = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal) =>
            {
                try
                {
                    eventTrigger(sboObject, pVal);
                }
                catch (Exception e)
                {
                    if (form != null && form.UIAPIRawForm != null)
                        form.UIAPIRawForm.Freeze(false); // force unfreeze in case of error.

                    Assembly addinAssembly = eventTrigger.Method.DeclaringType.Assembly;
                    Version objVersion = addinAssembly.GetName().Version;
                    String addInName = addinAssembly.GetName().Name;
                    String addInVersion = objVersion.Major.ToString() + "." + objVersion.Minor.ToString() + "." + objVersion.Build.ToString()
                                + "." + objVersion.Revision;

                    Logger.Error(String.Format(Messages.AddInError, addInName, addInVersion), e);
                }
            };
            return retFunction;
        }

        public static _IColumnEvents_ComboSelectAfterEventHandler ExceptionHandler(this _IColumnEvents_ComboSelectAfterEventHandler eventTrigger, FormBase form)
        {
            _IColumnEvents_ComboSelectAfterEventHandler retFunction = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal) =>
            {
                try
                {
                    eventTrigger(sboObject, pVal);
                }
                catch (Exception e)
                {
                    if (form != null && form.UIAPIRawForm != null)
                        form.UIAPIRawForm.Freeze(false); // force unfreeze in case of error.

                    Assembly addinAssembly = eventTrigger.Method.DeclaringType.Assembly;
                    Version objVersion = addinAssembly.GetName().Version;
                    String addInName = addinAssembly.GetName().Name;
                    String addInVersion = objVersion.Major.ToString() + "." + objVersion.Minor.ToString() + "." + objVersion.Build.ToString()
                                + "." + objVersion.Revision;

                    Logger.Error(String.Format(Messages.AddInError, addInName, addInVersion), e);
                }
            };
            return retFunction;
        }

        public static _IButtonEvents_ClickBeforeEventHandler ExceptionHandler(this _IButtonEvents_ClickBeforeEventHandler eventTrigger, FormBase form)
        {
            _IButtonEvents_ClickBeforeEventHandler retFunction = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) =>
            {
                BubbleEvent = true;
                try
                {
                    eventTrigger(sboObject, pVal, out BubbleEvent);
                }
                catch (Exception e)
                {
                    if (form != null && form.UIAPIRawForm != null)
                        form.UIAPIRawForm.Freeze(false); // force unfreeze in case of error.

                    Assembly addinAssembly = eventTrigger.Method.DeclaringType.Assembly;
                    Version objVersion = addinAssembly.GetName().Version;
                    String addInName = addinAssembly.GetName().Name;
                    String addInVersion = objVersion.Major.ToString() + "." + objVersion.Minor.ToString() + "." + objVersion.Build.ToString()
                                + "." + objVersion.Revision;

                    Logger.Error(String.Format(Messages.AddInError,addInName, addInVersion), e);                    
                }

            };

            return retFunction;
        }

        public static _IButtonComboEvents_ClickAfterEventHandler ExceptionHandler(this _IButtonComboEvents_ClickAfterEventHandler eventTrigger, FormBase form)
        {
            _IButtonComboEvents_ClickAfterEventHandler retFunction = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal) =>
            {
                try
                {
                    eventTrigger(sboObject, pVal);
                }
                catch (Exception e)
                {
                    if (form != null && form.UIAPIRawForm != null)
                        form.UIAPIRawForm.Freeze(false); // force unfreeze in case of error.

                    Assembly addinAssembly = eventTrigger.Method.DeclaringType.Assembly;
                    Version objVersion = addinAssembly.GetName().Version;
                    String addInName = addinAssembly.GetName().Name;
                    String addInVersion = objVersion.Major.ToString() + "." + objVersion.Minor.ToString() + "." + objVersion.Build.ToString()
                + "." + objVersion.Revision;

                    Logger.Error(String.Format(Messages.AddInError, addInName, addInVersion), e);
                }

            };

            return retFunction;
        }

        public static _IButtonComboEvents_ClickAfterEventHandler ChooseFromListHandler(this SAPbouiCOM.EditText targetEdit, SAPbouiCOM.UserDataSource ds)
        {
            _IButtonComboEvents_ClickAfterEventHandler handler = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal) =>
            {
                var cflE = (SAPbouiCOM.SBOChooseFromListEventArg)pVal;
                var oDT = cflE.SelectedObjects;
                if (oDT != null)
                {
                    ds.Value = oDT.GetValue(targetEdit.ChooseFromListAlias, 0).ToString();
                }

            };

            return handler.ExceptionHandler(null);
        }
    }
}
