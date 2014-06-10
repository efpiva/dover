using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Attribute;
using SAPbouiCOM;

namespace AddOne.Framework.DAO
{
    public abstract class BusinessOneUIDAO
    {
        public abstract void ProcessMenuAttribute(List<MenuAttribute> menus);

        internal abstract IForm CreateUserForm(string xml, string type);
    }
}
