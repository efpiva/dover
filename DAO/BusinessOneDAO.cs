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
using Dover.Framework.Model;
using Dover.Framework.Model.SAP;
using System.IO;
using SAPbobsCOM;

namespace Dover.Framework.DAO
{
    public interface INotifier
    {
        void OnSuccess(string xml, string addedKey);
        void OnError(string xml, int errCode, string errMessage);
    }

    public interface BusinessOneDAO
    {
        void SaveBOMIfNotExists(IBOM bom, INotifier notifier = null);

        void UpdateOrSaveBOMIfNotExists(IBOM udoBOM, INotifier notifier = null);

        /// <summary>
        /// Return a BOM XML, containing various BO elements.
        /// </summary>
        /// <typeparam name="V">SAP Business One DI API object type</typeparam>
        /// <param name="keys">keys to be fetched</param>
        /// <param name="objType">SAP Business One DI API enum representing the object type</param>
        /// <returns></returns>
        string GetXMLBom<V>(object[] keys, BoObjectTypes objType);

        List<string> ListMissingBOMKeys(IBOM userFieldBOM);

        string GetNextCode(String udt);

        string GetCurrentUser();

        void ExecuteStatement(string sql);

        T ExecuteSqlForObject<T>(string sql);

        List<T> ExecuteSqlForList<T>(string sql);

        T GetBOMFromXML<T>(Stream resourceStream);

        void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute);

        bool IsSuperUser();

        void SaveBOM(IBOM doc, INotifier notifier = null);

        bool PermissionExists(Attribute.PermissionAttribute permissionAttribute);
    }
}
