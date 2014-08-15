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

    /// <summary>
    /// Business One DAO (Data Access Object) Implementation. 
    /// </summary>
    public interface BusinessOneDAO
    {
        /// <summary>
        /// Save a BOM XML if not presented in the DataBase.
        /// </summary>
        /// <param name="bom">BOM, containing the XML that will be added.</param>
        /// <param name="notifier">
        ///  If present, for each attempt, a trigger event will happen. This is usefull
        ///  when the calling need to knows the added ID, of to do some transaction related stuff
        ///  OnError.
        /// </param>
        void SaveBOMIfNotExists(IBOM bom, INotifier notifier = null);

        /// <summary>
        /// Save a BOM if not exists and update it if exists.
        /// </summary>
        /// <param name="udoBOM"></param>
        /// <param name="notifier"></param>
        void UpdateOrSaveBOMIfNotExists(IBOM BOM, INotifier notifier = null);

        /// <summary>
        /// Return a BOM XML, containing various BO elements.
        /// </summary>
        /// <typeparam name="V">SAP Business One DI API object type</typeparam>
        /// <param name="keys">keys to be fetched</param>
        /// <param name="objType">SAP Business One DI API enum representing the object type</param>
        /// <returns>XML of SAP Business One object</returns>
        string GetXMLBom<V>(object[] keys, BoObjectTypes objType);

        /// <summary>
        /// List all elements that are not present on the database, from the specified BOM
        /// </summary>
        /// <param name="bom">BOM with all Business One objects</param>
        /// <returns>all index for the elements that are missing.</returns>
        List<int> ListMissingBOMKeys(IBOM bom);

        /// <summary>
        /// List all elements that are not present or outdated on the database, from the specified BOM
        /// </summary>
        /// <param name="bom">BOM with all Business Obe objects</param>
        /// <returns>all index for the elements that are missing or outdated.</returns>
        List<int> ListOutdatedBOMKeys(IBOM bom);

        /// <summary>
        /// Return the next code for the specified user table. This method implements a code on the following format:
        ///   XXXXXXXX, where X is an hex digit.;
        /// </summary>
        /// <param name="udt">User Defined Table</param>
        /// <returns>next code on the default format.</returns>
        string GetNextCode(String udt);

        /// <summary>
        /// Return the current user code.
        /// </summary>
        /// <returns>Return the current user code.</returns>
        string GetCurrentUser();

        /// <summary>
        /// Execute an SQL statement with no return value (INSERT/UPDATE).
        /// </summary>
        /// <param name="sql">SQL Statement</param>
        void ExecuteStatement(string sql);

        /// <summary>
        /// Execute the SQL Statement and return the supplied object. For each statement selected value,
        /// a corresponding property must exists on the specified object.
        /// </summary>
        /// <typeparam name="T">Return object type</typeparam>
        /// <param name="sql">SQL statement</param>
        /// <returns>instantiated object with all values returned on the SQL statement.</returns>
        T ExecuteSqlForObject<T>(string sql);

        /// <summary>
        /// Execute the SQL Statement and return a list of the supplied object type. For each statement selected value,
        /// a corresponding property must exists on the specified object.
        /// </summary>
        /// <typeparam name="T">Object type to wrap into the list.</typeparam>
        /// <param name="sql">SQL statement</param>
        /// <returns>List with an object for each row, with all values returned on the SQL statement.</returns>
        List<T> ExecuteSqlForList<T>(string sql);

        /// <summary>
        /// Return a BOM object with the specified resourceStream.
        /// </summary>
        /// <typeparam name="T">BOM type</typeparam>
        /// <param name="resourceStream">Stream containing XML data.</param>
        /// <returns>BOM Object representing the returned value.</returns>
        T GetBOMFromXML<T>(Stream resourceStream);

        /// <summary>
        /// Update a permission attribute on the database if not exists.
        /// </summary>
        /// <param name="permissionAttribute">PermissionAttribute</param>
        void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute);

        /// <summary>
        /// Return true if current iser us SuperUser.
        /// </summary>
        /// <returns>bool value indicating the current user is SuperUser.</returns>
        bool IsSuperUser();

        /// <summary>
        /// Saves BOM, without checking if it exists.
        /// </summary>
        /// <param name="doc">BOM object representing the BusinessOne data to be saved.</param>
        /// <param name="notifier">
        ///  If present, for each attempt, a trigger event will happen. This is usefull
        ///  when the calling need to knows the added ID, of to do some transaction related stuff
        ///  OnError.
        /// </param>
        void SaveBOM(IBOM doc, INotifier notifier = null);

        /// <summary>
        /// Check if the specified Permission exists.
        /// </summary>
        /// <param name="permissionAttribute">Permission attribute to check for</param>
        /// <returns>true if exists</returns>
        bool PermissionExists(Attribute.PermissionAttribute permissionAttribute);

        /// <summary>
        /// Return the desired Business Object API from current Company
        /// </summary>
        /// <param name="objType">Desired ObjectType</param>
        /// <returns></returns>
        dynamic GetBusinessObject(BoObjectTypes objType);

        /// <summary>
        /// Call the Update() API on the desired object. On Error an exception is thrown and the 
        /// Business Object is cleaned from memory (ReleaseComObject is called).
        /// </summary>
        /// <param name="b1Object">COM object that will have Update() call done.</param>
        void UpdateBusinessObject(object b1Object);

        /// <summary>
        /// Call the Save() API on the desired object. On Error an exception is thrown and the 
        /// Business Object is cleaned from memory (ReleaseComObject is called).
        /// </summary>
        /// <param name="b1Object">COM object that will have Save() call done.</param>
        void SaveBusinessObject(object b1Object);

        /// <summary>
        /// Proper release of COM resources.
        /// </summary>
        /// <param name="b1Object">COM object that will be released</param>
        void Release(object b1Object);
    }
}
