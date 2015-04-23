SELECT "Code" AS "AddinCode", "U_Name" AS "AddInName", "U_Status" AS "PermissionStr" 
FROM "@DOVER_MODULES" 
WHERE "U_Type" = 'A' and "U_DueDate" > current_timestamp