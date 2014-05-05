using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Attribute;

namespace AddOne.Framework.DAO
{
    internal abstract class BusinessOneUIDAO
    {
        internal abstract void ProcessMenuAttribute(List<MenuAttribute> menus);
    }
}
