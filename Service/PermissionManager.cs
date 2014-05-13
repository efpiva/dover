using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.DAO;

namespace AddOne.Framework.Service
{
    public class PermissionManager
    {

        private PermissionDAO permissionDAO;

        public PermissionManager(PermissionDAO permissionDAO)
        {
            this.permissionDAO = permissionDAO;
        }

        internal bool AddInEnabled(string addInName)
        {
            Permission perm;

            perm = permissionDAO.GetUserPermission(addInName);
            if (perm == Permission.Inactive)
                return false;
            else if (perm == Permission.Active)
                return true;
            else if (perm == Permission.Default)
            {
                perm = permissionDAO.GetAddInPermission(addInName);
                return (perm == Permission.Active);
            }

            return false;
        }
    }
}
