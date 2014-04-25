using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.Attribute
{
    public enum MenuType
    {
        Folder = 2,
        Item = 1
    }


    public class MenuAttribute : System.Attribute
    {
        public string Checked;
        public string Enabled;
        public string FatherUID;
        public string String;
        public MenuType Type;
        public string UniqueID;
        public string Image;

        public MenuAttribute(string FatherUID, string String, MenuType Type, string UniqueID,
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

    }
}
