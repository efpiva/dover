using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.Service;

namespace Dover.Framework.DAO
{
    public abstract class PermissionDAO
    {
        internal abstract Permission GetUserPermission(string addInName);

        internal abstract Permission GetAddInPermission(string addInName);

        internal abstract void SaveAddInPermission(string addInName, Permission permission);

        internal abstract string GetUserPermissionCode(string addInName, string userName);

        internal abstract void SaveAddInPermission(string addInName, string userName, Permission permission);

        internal abstract void UpdateAddInPermission(string userPermissionCode, Permission permission);
    }
}
