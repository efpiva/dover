SELECT T0.Code, T0.U_Name Name, ISNULL(U_Description, U_Name) Description, U_FileName FileName, U_Version Version, U_MD5 MD5, U_Date Date, 
                                U_Size Size, U_Type TypeCode 
                            FROM [@DOVER_MODULES] T0 inner join [@DOVER_MODULES_DEP] T1 on T0.Code = T1.U_DepCode
                            WHERE T1.U_Code = '{0}'
