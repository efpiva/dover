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
using Dover.Framework.Factory;
using Castle.Core;

namespace Dover.Framework.Form
{
    /// <summary>
    /// Base class for all forms. It has the same signature of FormBase from SAP Business One
    /// Business One Studio Framework.
    /// 
    /// It has a Proxy installed for all Forms (Dover.Framework.Proxy.FormProxy) that handle i18n
    /// and unexpected exceptions. To use the exception handled stuff, install all UI events
    /// declaring the method as protected virtual. All forms should be declared as Public.
    /// </summary>
    [Interceptor("formProxy")]
    public class DoverFormBase
    {
        protected SAPbouiCOM.Application app;

        internal DoverFormBase()
        {
            app = ContainerManager.Container.Resolve<SAPbouiCOM.Application>();
        }

        public void Show()
        {
            if (this.UIAPIRawForm != null)
                this.UIAPIRawForm.Visible = true;
        }

        protected Item GetItem(object key)
        {
            return this.UIAPIRawForm.Items.Item(key);
        }

        internal virtual string FormUID { private get; set; }

        public IForm UIAPIRawForm { get; internal set; }
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

        protected internal virtual void OnFormLoadBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.LoadBefore != null)
                this.LoadBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormLoadAfter(SBOItemEventArg pVal)
        {
            if (this.LoadAfter != null)
                this.LoadAfter(pVal);
        }

        protected internal virtual void OnFormUnloadBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.UnloadBefore != null)
                this.UnloadBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormUnloadAfter(SBOItemEventArg pVal)
        {
            if (this.UnloadAfter != null)
                this.UnloadAfter(pVal);
        }

        protected internal virtual void OnFormActivateBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.ActivateBefore != null)
                this.ActivateBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormActivateAfter(SBOItemEventArg pVal)
        {
            if (this.ActivateAfter != null)
                this.ActivateAfter(pVal);
        }

        protected internal virtual void OnFormDeactivateBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.DeactivateBefore != null)
                this.DeactivateBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormDeactivateAfter(SBOItemEventArg pVal)
        {
            if (this.DeactivateAfter != null)
                this.DeactivateAfter(pVal);
        }

        protected internal virtual void OnFormResizeBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.ResizeBefore != null)
                this.ResizeBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormResizeAfter(SBOItemEventArg pVal)
        {
            if (this.ResizeAfter != null)
                this.ResizeAfter(pVal);
        }

        protected internal virtual void OnFormMenuHighlightBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.MenuHighlightBefore != null)
                this.MenuHighlightBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormMenuHighlightAfter(SBOItemEventArg pVal)
        {
            if (this.MenuHighlightAfter != null)
                this.MenuHighlightAfter(pVal);
        }

        protected internal virtual void OnFormDataAddBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.DataAddBefore != null)
                this.DataAddBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormDataAddAfter(ref BusinessObjectInfo pVal)
        {
            if (this.DataAddAfter != null)
                this.DataAddAfter(ref pVal);
        }

        protected internal virtual void OnFormDataUpdateBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.DataUpdateBefore != null)
                this.DataUpdateBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormDataUpdateAfter(ref BusinessObjectInfo pVal)
        {
            if (this.DataUpdateAfter != null)
                this.DataUpdateAfter(ref pVal);
        }

        protected internal virtual void OnFormDataDeleteBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.DataDeleteBefore != null)
                this.DataDeleteBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormDataDeleteAfter(ref BusinessObjectInfo pVal)
        {
            if (this.DataDeleteAfter != null)
                this.DataDeleteAfter(ref pVal);
        }

        protected internal virtual void OnFormDataLoadBefore(ref BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.DataLoadBefore != null)
                this.DataLoadBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormDataLoadAfter(ref BusinessObjectInfo pVal)
        {
            if (this.DataLoadAfter != null)
                this.DataLoadAfter(ref pVal);
        }

        protected internal virtual void OnFormKeyDownBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.KeyDownBefore != null)
                this.KeyDownBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormKeyDownAfter(SBOItemEventArg pVal)
        {
            if (this.KeyDownAfter != null)
                this.KeyDownAfter(pVal);
        }

        protected internal virtual void OnFormLayoutKeyBefore(ref LayoutKeyInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.LayoutKeyBefore != null)
                this.LayoutKeyBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormLayoutKeyAfter(ref LayoutKeyInfo pVal)
        {
            if (this.LayoutKeyAfter != null)
                this.LayoutKeyAfter(ref pVal);
        }

        protected internal virtual void OnFormPrintBefore(ref PrintEventInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.PrintBefore != null)
                this.PrintBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormPrintAfter(ref PrintEventInfo pVal)
        {
            if (this.PrintAfter != null)
                this.PrintAfter(ref pVal);
        }

        protected internal virtual void OnFormReportDataBefore(ref PrintEventInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.ReportDataBefore != null)
                this.ReportDataBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormReportDataAfter(ref PrintEventInfo pVal)
        {
            if (this.ReportDataAfter != null)
                this.ReportDataAfter(ref pVal);
        }

        protected internal virtual void OnFormClickBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.ClickBefore != null)
                this.ClickBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormClickAfter(SBOItemEventArg pVal)
        {
            if (this.ClickAfter != null)
                this.ClickAfter(pVal);
        }

        protected internal virtual void OnFormRightClickBefore(ref ContextMenuInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.RightClickBefore != null)
                this.RightClickBefore(ref pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormRightClickAfter(ref ContextMenuInfo pVal)
        {
            if (this.RightClickAfter != null)
                this.RightClickAfter(ref pVal);
        }

        protected internal virtual void OnFormCloseBefore(SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (this.CloseBefore != null)
                this.CloseBefore(pVal, out BubbleEvent);
        }

        protected internal virtual void OnFormCloseAfter(SBOItemEventArg pVal)
        {
            if (this.CloseAfter != null)
                this.CloseAfter(pVal);
        }

        protected internal virtual void OnFormVisibleAfter(SBOItemEventArg pVal)
        {
            if (this.VisibleAfter != null)
                this.VisibleAfter(pVal);
        }

        public virtual void OnInitializeComponent()
        {
        }

        public virtual void OnInitializeFormEvents()
        {
        }
    }
}
