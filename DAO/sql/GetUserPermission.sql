SELECT [@DOVER_MODULES].U_Name AddInName, 
                            case ISNULL([@DOVER_MODULES_USER].U_Status, [@DOVER_MODULES].U_Status) when 'D' then [@DOVER_MODULES].U_Status
                                    else ISNULL([@DOVER_MODULES_USER].U_Status, [@DOVER_MODULES].U_Status) end PermissionStr
                     from [@DOVER_MODULES]
                                            LEFT JOIN [@DOVER_MODULES_USER] ON [@DOVER_MODULES].Code = [@DOVER_MODULES_USER].U_Code and [@DOVER_MODULES_USER].U_User = '{0}'
                                where [@DOVER_MODULES].U_Type = 'A' 
                    and ([@DOVER_MODULES_USER].U_User is null or [@DOVER_MODULES_USER].U_User = '{0}')