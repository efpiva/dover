SELECT [@DOVER_MODULES].U_Description Description, 
                                        [@DOVER_MODULES].U_Version Version, COALESCE([@DOVER_MODULES_USER].U_Status,
                                        'D') Status, [@DOVER_MODULES].Code, [@DOVER_MODULES].U_Name Name
                        from [@DOVER_MODULES] LEFT JOIN [@DOVER_MODULES_USER] ON [@DOVER_MODULES].Code = [@DOVER_MODULES_USER].U_Code and [@DOVER_MODULES_USER].U_User = '{0}'
                        where [@DOVER_MODULES].U_Type = 'A' and ([@DOVER_MODULES_USER].U_User is null or [@DOVER_MODULES_USER].U_User = '{0}')