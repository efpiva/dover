using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using AddOne.Framework.Monad;
using AddOne.Framework.Service;

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
                "SELECT U_Name AddInName, U_Status PermissionStr from [@GA_AO_MODULES] where U_Type = 'A'"
                );
            foreach (var permission in addInPermission)
            {
                addInHash.Add(permission.AddInName, permission.Permission);
                Logger.Debug(String.Format(Messages.AddInPermission, permission.AddInName, permission.Permission));
            }
            string currentUser = b1DAO.GetCurrentUser();
            addInPermission = b1DAO.ExecuteSqlForList<AddInPermission>(
                String.Format(@"SELECT [@GA_AO_MODULES].U_Name AddInName, 
                            case ISNULL([@GA_AO_MODULES_USER].U_Status, [@GA_AO_MODULES].U_Status) when 'D' then [@GA_AO_MODULES].U_Status
                                    else ISNULL([@GA_AO_MODULES_USER].U_Status, [@GA_AO_MODULES].U_Status) end PermissionStr
                     from [@GA_AO_MODULES]
                                            LEFT JOIN [@GA_AO_MODULES_USER] ON [@GA_AO_MODULES].Code = [@GA_AO_MODULES_USER].U_Code and [@GA_AO_MODULES_USER].U_User = '{0}'
                                where [@GA_AO_MODULES].U_Type = 'A' 
                    and ([@GA_AO_MODULES_USER].U_User is null or [@GA_AO_MODULES_USER].U_User = '{0}')", currentUser)
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


        public void SaveAddInPermission(string addInName, Permission permission)
        {
            b1DAO.ExecuteStatement(string.Format(@"UPDATE [@GA_AO_MODULES] Set U_Status = '{0}' WHERE U_Name = '{1}'",
                GetPermissionStr(permission), addInName));
        }

        public string GetUserPermissionCode(string addInName, string userName)
        {
            var moduleCode = b1DAO.ExecuteSqlForObject<string>(string.Format(
                "SELECT Code FROM [@GA_AO_MODULES] WHERE U_Name = '{0}'", addInName));
            return  b1DAO.ExecuteSqlForObject<string>(string.Format(
                "SELECT Code from [@GA_AO_MODULES_USER] where U_Code = '{0}' and U_User = '{1}'", moduleCode, userName));
        }

        public void SaveAddInPermission(string addInName, string userName, Permission permission)
        {
            var moduleCode = b1DAO.ExecuteSqlForObject<string>(string.Format(
                "SELECT Code FROM [@GA_AO_MODULES] WHERE U_Name = '{0}'", addInName));
            var nextCode = b1DAO.GetNextCode("GA_AO_MODULES_USER");
            b1DAO.ExecuteStatement(string.Format(@"INSERT INTO [@GA_AO_MODULES_USER] (Code, Name, U_Code, U_Status, U_User)
                VALUES ('{0}', '{0}', '{1}', '{2}', '{3}')", nextCode, moduleCode, GetPermissionStr(permission), userName));
        }

        public void UpdateAddInPermission(string userPermissionCode, Permission permission)
        {
            b1DAO.ExecuteStatement(string.Format(
                "UPDATE [@GA_AO_MODULES_USER] Set U_Status = '{0}' where Code = '{1}'", GetPermissionStr(permission), userPermissionCode));
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
