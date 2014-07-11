using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace Dover.Framework.Attribute
{

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class MenuAttribute : System.Attribute, IComparable<MenuAttribute>
    {
        public string Checked = "0";
        public string Enabled = "1";
        public string FatherUID;
        public string String;
        public BoMenuType Type;
        public string UniqueID;
        public string Image;
        public string ValidateMethod;
        public string i18n;
        public int Order;

        public MenuAttribute() { }

        public MenuAttribute(string FatherUID, string String, BoMenuType Type, string UniqueID,
            string Image = null, string Checked = "0", string Enabled = "1")
        {
            this.Checked = Checked;
            this.Enabled = Enabled;
            this.FatherUID = FatherUID;
            this.String = String;
            this.Type = Type;
            this.UniqueID = UniqueID;
            this.Image = Image;
        }


        internal System.Type OriginalType { get; set; }

        public int CompareTo(MenuAttribute other)
        {
            return this.Order.CompareTo(other.Order);
        }
    }
}
