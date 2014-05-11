using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.Core.Logging;
using SAPbouiCOM.Framework;

namespace AddOne.Framework.Form
{
    public class EventHandler
    {

        public ILogger Logger { get; set; }

        public delegate void InternalEvent(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void InternalAfterEvent(object sboObject, SAPbouiCOM.SBOItemEventArg pVal);

        public InternalEvent ExceptionHandler(InternalEvent eventTrigger, FormBase form)
        {
            InternalEvent retFunction = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent) =>
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

        public InternalAfterEvent ExceptionHandler(InternalAfterEvent eventTrigger, FormBase form)
        {
            InternalAfterEvent retFunction = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal) =>
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

        public InternalAfterEvent ChooseFromListHandler(SAPbouiCOM.EditText targetEdit, SAPbouiCOM.UserDataSource ds)
        {
            InternalAfterEvent handler = (object sboObject, SAPbouiCOM.SBOItemEventArg pVal) =>
            {
                var cflE = (SAPbouiCOM.SBOChooseFromListEventArg)pVal;
                var oDT = cflE.SelectedObjects;
                if (oDT != null)
                {
                    ds.Value = oDT.GetValue(targetEdit.ChooseFromListAlias, 0).ToString();
                }

            };

            return ExceptionHandler(handler, null);
        }
    }
}
