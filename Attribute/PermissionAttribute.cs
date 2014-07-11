using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;

namespace Dover.Framework.Attribute
{    
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class PermissionAttribute : System.Attribute
    {
        public String PermissionID { get; set; }
        public String Name { get; set; }
        public String ParentID { get; set; }        
        public String FormType { get; set; }
        public BoUPTOptions Options { get; set; }

        public PermissionAttribute(string PermissionID, string Name, string ParentID,
            string FormType = "", BoUPTOptions Options = BoUPTOptions.bou_FullNone)
        {
            this.PermissionID = PermissionID;
            this.Name = Name;
            this.ParentID = ParentID;
            this.FormType = FormType;
            this.Options = Options;
        }
    }
}
