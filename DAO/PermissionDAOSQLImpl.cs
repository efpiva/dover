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
using Castle.Core.Logging;
using Dover.Framework.Monad;
using Dover.Framework.Service;
using Dover.Framework.Log;

namespace Dover.Framework.DAO
{

    internal class PermissionDAOSQLImpl : PermissionDAO
    {

        public ILogger Logger { get; set; }

        public class AddInPermission
        {
            public String AddInName { get; set; }
            public Permission Permission
            {
                get
                {
                    switch (PermissionStr.Return(x => x, "A") )
                    {
                        case "A":
                            return Permission.Active;
                        case "I":
                            return Permission.Inactive;
                        default:
                            return Permission.Default;
                    }
                }
            }
            public string PermissionStr { get; set; }
        }

        private BusinessOneDAO b1DAO;
        private Dictionary<string, Permission> addInHash = new Dictionary<string, Permission>();
        private Dictionary<string, Permission> userAddInHash = new Dictionary<string, Permission>();

        public PermissionDAOSQLImpl(BusinessOneDAO b1DAO, ILogger Logger)
        {
            this.Logger = Logger;
            this.b1DAO = b1DAO;
            List<AddInPermission> addInPermission = b1DAO.ExecuteSqlForList<AddInPermission>(
                this.GetSQL("GetModulePermission.sql"));
            foreach (var permission in addInPermission)
            {
                addInHash.Add(permission.AddInName, permission.Permission);
                Logger.Debug(DebugString.Format(Messages.AddInPermission, permission.AddInName, permission.Permission));
            }
            string currentUser = b1DAO.GetCurrentUser();
            addInPermission = b1DAO.ExecuteSqlForList<AddInPermission>(
                String.Format(this.GetSQL("GetUserPermission.sql"), currentUser)
                );
            foreach (var permission in addInPermission)
            {
                userAddInHash.Add(permission.AddInName, permission.Permission);
                Logger.Debug(DebugString.Format(Messages.AddInUserPermission, currentUser, permission.AddInName, permission.Permission));
            }
        }

        internal override Permission GetUserPermission(string addInName)
        {
            Permission value;
            userAddInHash.TryGetValue(addInName, out value);
            return value;
        }

        internal override Permission GetAddInPermission(string addInName)
        {
            Permission value;
            addInHash.TryGetValue(addInName, out value);
            return value;
        }


        internal override void SaveAddInPermission(string addInName, Permission permission)
        {
            b1DAO.ExecuteStatement(string.Format(this.GetSQL("SaveAddinPermission.sql"),
                GetPermissionStr(permission), addInName));
        }

        internal override string GetUserPermissionCode(string addInName, string userName)
        {
            var moduleCode = b1DAO.ExecuteSqlForObject<string>(string.Format(
                this.GetSQL("GetModuleCode.sql"), addInName));
            return  b1DAO.ExecuteSqlForObject<string>(string.Format(
                this.GetSQL("GetUserPermissionCode.sql"), moduleCode, userName));
        }

        internal override void SaveAddInPermission(string addInName, string userName, Permission permission)
        {
            var moduleCode = b1DAO.ExecuteSqlForObject<string>(string.Format(
                this.GetSQL("GetModuleCode.sql"), addInName));
            var nextCode = b1DAO.GetNextCode("DOVER_MODULES_USER");
            b1DAO.ExecuteStatement(string.Format(this.GetSQL("SaveAddinUserPermission.sql"), nextCode, moduleCode, GetPermissionStr(permission), userName));
        }

        internal override void UpdateAddInPermission(string userPermissionCode, Permission permission)
        {
            b1DAO.ExecuteStatement(string.Format(
                this.GetSQL("UpdateAddinPermission.sql"), GetPermissionStr(permission), userPermissionCode));
        }

        private string GetPermissionStr(Permission permission)
        {
            switch (permission)
            {
                case Permission.Active:
                    return "A";
                case Permission.Inactive:
                    return "I";
                default:
                    return "D";
            }
        }
    }
}
