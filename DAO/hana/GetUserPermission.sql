SELECT "@DOVER_MODULES"."Code" AS "AddinCode", "@DOVER_MODULES"."U_Name" AS "AddInName", 
    CASE IFNULL("@DOVER_MODULES_USER"."U_Status", "@DOVER_MODULES"."U_Status") 
        WHEN 'D' THEN "@DOVER_MODULES"."U_Status" 
        ELSE IFNULL("@DOVER_MODULES_USER"."U_Status", "@DOVER_MODULES"."U_Status") 
    END AS "PermissionStr" 
FROM "@DOVER_MODULES" 
    LEFT OUTER JOIN "@DOVER_MODULES_USER" ON "@DOVER_MODULES"."Code" = "@DOVER_MODULES_USER"."U_Code" AND
         "@DOVER_MODULES_USER"."U_User" = '{0}' 
WHERE "@DOVER_MODULES"."U_Type" = 'A' AND ("@DOVER_MODULES_USER"."U_User" IS NULL OR
     "@DOVER_MODULES_USER"."U_User" = '{0}') AND "@DOVER_MODULES"."U_DueDate" > current_timestamp