using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Attribute;

namespace AddOne.Framework.DAO
{
    interface BusinessOneUIDAO
    {
        void ProcessMenuAttribute(List<MenuAttribute> menus);
    }
}
