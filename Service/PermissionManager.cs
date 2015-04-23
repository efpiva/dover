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
using Dover.Framework.DAO;
using Dover.Framework.Monad;

namespace Dover.Framework.Service
{

    public enum Permission
    {
        Inactive,
        Active,
        Default
    }

    internal class PermissionManager
    {

        private PermissionDAO permissionDAO;

        public PermissionManager(PermissionDAO permissionDAO)
        {
            this.permissionDAO = permissionDAO;
        }

        internal bool AddInEnabled(string addinCode)
        {
            Permission perm;

            perm = permissionDAO.GetUserPermission(addinCode);
            if (perm == Permission.Inactive)
                return false;
            else if (perm == Permission.Active)
                return true;
            else if (perm == Permission.Default)
            {
                perm = permissionDAO.GetAddInPermission(addinCode);
                return (perm == Permission.Active);
            }

            return false;
        }

        internal void ConfigureAddIn(string addinCode, Permission permission)
        {
            permissionDAO.SaveAddInPermission(addinCode, permission);
        }

        internal Permission ParsePermissionStr(string permissionStr)
        {
            switch (permissionStr.Return(x => x, "A"))
            {
                case "A":
                    return Permission.Active;
                case "I":
                    return Permission.Inactive;
                default:
                    return Permission.Default;
            }
        }

        internal void ConfigureAddIn(string addinCode, string userName, Permission permission)
        {
            string userPermissionCode = permissionDAO.GetUserPermissionCode(addinCode, userName);
            if (userPermissionCode == null)
                permissionDAO.SaveAddInPermission(addinCode, userName, permission);
            else
                permissionDAO.UpdateAddInPermission(userPermissionCode, permission);
        }

    }
}
