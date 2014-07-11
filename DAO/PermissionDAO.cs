using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Service;

namespace Dover.Framework.DAO
{
    public interface PermissionDAO
    {
        Permission GetUserPermission(string addInName);

        Permission GetAddInPermission(string addInName);

        void SaveAddInPermission(string addInName, Permission permission);

        string GetUserPermissionCode(string addInName, string userName);

        void SaveAddInPermission(string addInName, string userName, Permission permission);

        void UpdateAddInPermission(string userPermissionCode, Permission permission);
    }
}
