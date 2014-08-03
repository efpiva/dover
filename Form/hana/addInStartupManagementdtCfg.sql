SELECT "@DOVER_MODULES"."U_Description" AS "Description", "@DOVER_MODULES"."U_Version" AS "Version", 
    COALESCE("@DOVER_MODULES_USER"."U_Status", 'D') AS "Status", "@DOVER_MODULES"."Code", 
    "@DOVER_MODULES"."U_Name" AS "Name" 
FROM "@DOVER_MODULES" 
    LEFT OUTER JOIN "@DOVER_MODULES_USER" ON "@DOVER_MODULES"."Code" = "@DOVER_MODULES_USER"."U_Code" AND
         "@DOVER_MODULES_USER"."U_User" = 'manager' 
WHERE "@DOVER_MODULES"."U_Type" = 'A' AND ("@DOVER_MODULES_USER"."U_User" IS NULL OR
     "@DOVER_MODULES_USER"."U_User" = 'manager')