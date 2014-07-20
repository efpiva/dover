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
using System.Collections.Generic;
using Dover.Framework.Attribute;
using SAPbouiCOM;

namespace Dover.Framework.DAO
{
    public abstract class BusinessOneUIDAO
    {
        internal abstract void ProcessMenuAttribute(List<MenuAttribute> menus);

        internal abstract IForm LoadFormBatchAction(string xml, string formType);

        internal abstract void LoadBatchAction(string xml);

        internal abstract IForm GetFormByUID(string formUID);
    }
}
