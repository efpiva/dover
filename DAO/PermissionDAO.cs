using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddOne.Framework.DAO
{

    public enum Permission
    {
        Inactive,
        Active,
        Default
    }

    public interface PermissionDAO
    {
        Permission GetUserPermission(string addInName);

        Permission GetAddInPermission(string addInName);
    }
}
