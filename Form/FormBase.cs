using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace AddOne.Framework.Form
{
    public class AddOneFormBase
    {
        public bool Alive { get; internal set; }

        public bool Initialized { get; internal set; }

        public event ActivateAfterHandler ActivateAfter;
        public event ActivateBeforeHandler ActivateBefore;
        public event ClickAfterHandler ClickAfter;
        public event ClickBeforeHandler ClickBefore;
        public event CloseAfterHandler CloseAfter;
        public event CloseBeforeHandler CloseBefore;
        public event DataAddAfterHandler DataAddAfter;
        public event DataAddBeforeHandler DataAddBefore;
        public event DataDeleteAfterHandler DataDeleteAfter;
        public event DataDeleteBeforeHandler DataDeleteBefore;
        public event DataLoadAfterHandler DataLoadAfter;
        public event DataLoadBeforeHandler DataLoadBefore;
        public event DataUpdateAfterHandler DataUpdateAfter;
        public event DataUpdateBeforeHandler DataUpdateBefore;
        public event DeactivateAfterHandler DeactivateAfter;
        public event DeactivateBeforeHandler DeactivateBefore;
        public event KeyDownAfterHandler KeyDownAfter;
        public event KeyDownBeforeHandler KeyDownBefore;
        public event LayoutKeyAfterHandler LayoutKeyAfter;
        public event LayoutKeyBeforeHandler LayoutKeyBefore;
        public event LoadAfterHandler LoadAfter;
        public event LoadBeforeHandler LoadBefore;
        public event MenuHighlightAfterHandler MenuHighlightAfter;
        public event MenuHighlightBeforeHandler MenuHighlightBefore;
        public event PrintAfterHandler PrintAfter;
        public event PrintBeforeHandler PrintBefore;
        public event ReportDataAfterHandler ReportDataAfter;
        public event ReportDataBeforeHandler ReportDataBefore;
        public event ResizeAfterHandler ResizeAfter;
        public event ResizeBeforeHandler ResizeBefore;
        public event RightClickAfterHandler RightClickAfter;
        public event RightClickBeforeHandler RightClickBefore;
        public event UnloadAfterHandler UnloadAfter;
        public event UnloadBeforeHandler UnloadBefore;
        public event VisibleAfterHandler VisibleAfter;

        public delegate void ActivateAfterHandler(SBOItemEventArg pVal);
        public delegate void ActivateBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void ClickAfterHandler(SBOItemEventArg pVal);
        public delegate void ClickBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void CloseAfterHandler(SBOItemEventArg pVal);
        public delegate void CloseBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void DataAddAfterHandler(ref BusinessObjectInfo pVal);
        public delegate void DataAddBeforeHandler(ref BusinessObjectInfo pVal, out bool BubbleEvent);
        public delegate void DataDeleteAfterHandler(ref BusinessObjectInfo pVal);
        public delegate void DataDeleteBeforeHandler(ref BusinessObjectInfo pVal, out bool BubbleEvent);
        public delegate void DataLoadAfterHandler(ref BusinessObjectInfo pVal);
        public delegate void DataLoadBeforeHandler(ref BusinessObjectInfo pVal, out bool BubbleEvent);
        public delegate void DataUpdateAfterHandler(ref BusinessObjectInfo pVal);
        public delegate void DataUpdateBeforeHandler(ref BusinessObjectInfo pVal, out bool BubbleEvent);
        public delegate void DeactivateAfterHandler(SBOItemEventArg pVal);
        public delegate void DeactivateBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void KeyDownAfterHandler(SBOItemEventArg pVal);
        public delegate void KeyDownBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void LayoutKeyAfterHandler(ref LayoutKeyInfo eventInfo);
        public delegate void LayoutKeyBeforeHandler(ref LayoutKeyInfo eventInfo, out bool BubbleEvent);
        public delegate void LoadAfterHandler(SBOItemEventArg pVal);
        public delegate void LoadBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void MenuHighlightAfterHandler(SBOItemEventArg pVal);
        public delegate void MenuHighlightBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void PrintAfterHandler(ref PrintEventInfo eventInfo);
        public delegate void PrintBeforeHandler(ref PrintEventInfo eventInfo, out bool BubbleEvent);
        public delegate void ReportDataAfterHandler(ref PrintEventInfo eventInfo);
        public delegate void ReportDataBeforeHandler(ref PrintEventInfo eventInfo, out bool BubbleEvent);
        public delegate void ResizeAfterHandler(SBOItemEventArg pVal);
        public delegate void ResizeBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void RightClickAfterHandler(ref ContextMenuInfo eventInfo);
        public delegate void RightClickBeforeHandler(ref ContextMenuInfo eventInfo, out bool BubbleEvent);
        public delegate void UnloadAfterHandler(SBOItemEventArg pVal);
        public delegate void UnloadBeforeHandler(SBOItemEventArg pVal, out bool BubbleEvent);
        public delegate void VisibleAfterHandler(SBOItemEventArg pVal);
    }
}
