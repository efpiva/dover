using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using AddOne.Framework.Monad;

namespace AddOne.Framework.DAO
{

    public class PermissionDAOSQLImpl : PermissionDAO
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
                            return DAO.Permission.Active;
                        case "I":
                            return DAO.Permission.Inactive;
                        default:
                            return DAO.Permission.Default;
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
                "SELECT U_Name AddInName, U_Status PermissionStr from [@GA_AO_MODULES] where U_Type = 'A'"
                );
            foreach (var permission in addInPermission)
            {
                addInHash.Add(permission.AddInName, permission.Permission);
                Logger.Debug(String.Format(Messages.AddInPermission, permission.AddInName, permission.Permission));
            }
            string currentUser = b1DAO.GetCurrentUser();
            addInPermission = b1DAO.ExecuteSqlForList<AddInPermission>(
                String.Format(@"SELECT [@GA_AO_MODULES].U_Name AddInName, [@GA_AO_MODULES_USER].U_Status PermissionStr from [@GA_AO_MODULES]
                        INNER JOIN [@GA_AO_MODULES_USER] ON [@GA_AO_MODULES].Code = [@GA_AO_MODULES_USER].U_Code
            where [@GA_AO_MODULES].U_Type = 'A' and [@GA_AO_MODULES_USER].U_User = '{0}'", currentUser)
                );
            foreach (var permission in addInPermission)
            {
                userAddInHash.Add(permission.AddInName, permission.Permission);
                Logger.Debug(String.Format(Messages.AddInUserPermission, currentUser, permission.AddInName, permission.Permission));
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
