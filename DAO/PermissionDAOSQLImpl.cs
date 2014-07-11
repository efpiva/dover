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
                "SELECT U_Name AddInName, U_Status PermissionStr from [@DOVER_MODULES] where U_Type = 'A'"
                );
            foreach (var permission in addInPermission)
            {
                addInHash.Add(permission.AddInName, permission.Permission);
                Logger.Debug(DebugString.Format(Messages.AddInPermission, permission.AddInName, permission.Permission));
            }
            string currentUser = b1DAO.GetCurrentUser();
            addInPermission = b1DAO.ExecuteSqlForList<AddInPermission>(
                String.Format(@"SELECT [@DOVER_MODULES].U_Name AddInName, 
                            case ISNULL([@DOVER_MODULES_USER].U_Status, [@DOVER_MODULES].U_Status) when 'D' then [@DOVER_MODULES].U_Status
                                    else ISNULL([@DOVER_MODULES_USER].U_Status, [@DOVER_MODULES].U_Status) end PermissionStr
                     from [@DOVER_MODULES]
                                            LEFT JOIN [@DOVER_MODULES_USER] ON [@DOVER_MODULES].Code = [@DOVER_MODULES_USER].U_Code and [@DOVER_MODULES_USER].U_User = '{0}'
                                where [@DOVER_MODULES].U_Type = 'A' 
                    and ([@DOVER_MODULES_USER].U_User is null or [@DOVER_MODULES_USER].U_User = '{0}')", currentUser)
                );
            foreach (var permission in addInPermission)
            {
                userAddInHash.Add(permission.AddInName, permission.Permission);
                Logger.Debug(DebugString.Format(Messages.AddInUserPermission, currentUser, permission.AddInName, permission.Permission));
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
            b1DAO.ExecuteStatement(string.Format(@"UPDATE [@DOVER_MODULES] Set U_Status = '{0}' WHERE U_Name = '{1}'",
                GetPermissionStr(permission), addInName));
        }

        public string GetUserPermissionCode(string addInName, string userName)
        {
            var moduleCode = b1DAO.ExecuteSqlForObject<string>(string.Format(
                "SELECT Code FROM [@DOVER_MODULES] WHERE U_Name = '{0}'", addInName));
            return  b1DAO.ExecuteSqlForObject<string>(string.Format(
                "SELECT Code from [@DOVER_MODULES_USER] where U_Code = '{0}' and U_User = '{1}'", moduleCode, userName));
        }

        public void SaveAddInPermission(string addInName, string userName, Permission permission)
        {
            var moduleCode = b1DAO.ExecuteSqlForObject<string>(string.Format(
                "SELECT Code FROM [@DOVER_MODULES] WHERE U_Name = '{0}'", addInName));
            var nextCode = b1DAO.GetNextCode("DOVER_MODULES_USER");
            b1DAO.ExecuteStatement(string.Format(@"INSERT INTO [@DOVER_MODULES_USER] (Code, Name, U_Code, U_Status, U_User)
                VALUES ('{0}', '{0}', '{1}', '{2}', '{3}')", nextCode, moduleCode, GetPermissionStr(permission), userName));
        }

        public void UpdateAddInPermission(string userPermissionCode, Permission permission)
        {
            b1DAO.ExecuteStatement(string.Format(
                "UPDATE [@DOVER_MODULES_USER] Set U_Status = '{0}' where Code = '{1}'", GetPermissionStr(permission), userPermissionCode));
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
