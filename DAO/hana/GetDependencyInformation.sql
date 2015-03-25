SELECT "T0"."Code", "T0"."U_Name" AS "Name", IFNULL("U_Description", "U_Name") AS "Description", "U_FileName" AS "FileName", 
    "U_Version" AS "Version", U_MD5 AS "MD5", "U_Date" AS "Date", "U_Size" AS "Size", "U_Type" AS "TypeCode" 
FROM "@DOVER_MODULES" "T0" INNER JOIN "@DOVER_MODULES_DEP" "T1" on "T0"."Code" = "T1"."U_DepCode"
                            WHERE "T1"."U_Code" = '{0}'
