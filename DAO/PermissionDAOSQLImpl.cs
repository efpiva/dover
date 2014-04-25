using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.DAO
{

    class PermissionDAOSQLImpl : PermissionDAO
    {

        internal class AddInPermission
        {
            internal String AddInName { get; set; }
            internal Permission Permission { get; set; }
        }

        private BusinessOneDAO b1DAO;
        private Dictionary<string, Permission> addInHash = new Dictionary<string, Permission>();
        private Dictionary<string, Permission> userAddInHash = new Dictionary<string, Permission>();

        public PermissionDAOSQLImpl(BusinessOneDAO b1DAO)
        {
            this.b1DAO = b1DAO;
            List<AddInPermission> addInPermission = b1DAO.ExecuteSqlForList<AddInPermission>(
                "SELECT U_Name AddInName, U_Status Permission from [@GA_AO_MODULES] where U_Type = 'A'"
                );
            foreach (var permission in addInPermission)
            {
                addInHash.Add(permission.AddInName, permission.Permission);
            }
            string currentUser = b1DAO.GetCurrentUser();
            addInPermission = b1DAO.ExecuteSqlForList<AddInPermission>(
                String.Format(@"SELECT [@GA_AO_MODULES].U_Name AddInName, [@GA_AO_MODULES_USER].U_Status Permission from [@GA_AO_MODULES]
                        INNER JOIN [@GA_AO_MODULES_USER] ON [@GA_AO_MODULES].Code = [@GA_AO_MODULES_USER].U_Code
            where [@GA_AO_MODULES].U_Type = 'A' and [@GA_AO_MODULES_USER].U_User = '{0}'", currentUser)
                );
            foreach (var permission in addInPermission)
            {
                userAddInHash.Add(permission.AddInName, permission.Permission);
            }
        }

        public Permission GetUserPermission(string addInName)
        {
            Permission value;
            userAddInHash.TryGetValue(addInName, out value);
            return value;
        }

        public Permission GetAddInPermission(string addInName)
        {
            Permission value;
            addInHash.TryGetValue(addInName, out value);
            return value;
        }
    }
}
