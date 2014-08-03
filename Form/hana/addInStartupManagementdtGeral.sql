SELECT T0."U_Name" AS "Name", T0."U_Description" AS "Description", T0."U_Version" AS "Version", 
    IFNULL(T0."U_Status", 'A') AS "Status", "Code" 
FROM "@DOVER_MODULES" T0 
WHERE "U_Type" = 'A'