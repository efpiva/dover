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

namespace Dover.Framework.Attribute
{

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class MenuAttribute : System.Attribute, IComparable<MenuAttribute>
    {
        /// <summary>
        /// Indicates whether the menu item gets a check mark when clicked.
        /// </summary>
        public string Checked = "0";
        /// <summary>
        /// Indicates whether the menu item is enabled.
        /// </summary>
        public string Enabled = "1";
        /// <summary>
        /// UID of the top level menu
        /// </summary>
        public string FatherUID;
        /// <summary>
        /// The display name of the menu item.
        /// </summary>
        public string String;
        /// <summary>
        /// The type of menu item.
        /// mt_STRING - active menu item with no subitems (menu ID should be greater than 0)
        /// mt_POPUP - Menu item with subitems
        /// mt_SEPERATOR - Separator item
        /// </summary>
        public BoMenuType Type;
        /// <summary>
        /// The unique ID of the menu item.
        /// </summary>
        public string UniqueID;
        /// <summary>
        /// The path to an image file to be displayed to the left of the menu item.
        /// </summary>
        public string Image = "";
        /// <summary>
        /// The name of a method, in the current class, that will indicates at runtime if the menu should be
        /// enabled or disabled. This method can, for example, check user permissions or check if user is super-user.
        /// 
        /// the method should return bool and have no parameters, so as an example:
        /// 
        /// public bool MyMethod();
        /// 
        /// This parameter would be: MyMethod.
        /// </summary>
        public string ValidateMethod;
        /// <summary>
        /// A resource full qualified name to be used as menu String.
        /// </summary>
        public string i18n;
        /// <summary>
        /// The position within the menu to place the menu item (1-based).
        /// </summary>
        public int Position;

        public MenuAttribute() { }

        /// <summary>
        /// Add a Menu Item to SAP Bussiness One Menus.
        /// </summary>
        /// <param name="FatherUID">UID of the top level menu</param>
        /// <param name="String">The display name of the menu item.</param>
        /// <param name="Type">The type of menu item.
        /// mt_STRING - active menu item with no subitems (menu ID should be greater than 0)
        /// mt_POPUP - Menu item with subitems
        /// mt_SEPERATOR - Separator item
        /// </param>
        /// <param name="UniqueID">The unique ID of the menu item.</param>
        /// <param name="Image">The path to an image file to be displayed to the left of the menu item.</param>
        /// <param name="Checked">Indicates whether the menu item gets a check mark when clicked.</param>
        /// <param name="Enabled">Indicates whether the menu item is enabled.</param>
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
            SAPbouiCOM.Application app = SAPServiceFactory.ApplicationFactory();

            bool thisExists = app.Menus.Exists(this.FatherUID);
            bool otherExists = app.Menus.Exists(other.FatherUID);

            if (thisExists && !otherExists)
                return -1;

            if (otherExists && !thisExists)
                return 1;

            if (this.FatherUID == other.FatherUID)
               return this.Position.CompareTo(other.Position);

            return this.UniqueID.CompareTo(other.UniqueID);
        }
    }
}
