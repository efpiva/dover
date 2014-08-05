SELECT "U_Name" AS "Name", "U_Version" AS "Version", 
    CASE 
        WHEN "U_Type" = 'C' THEN 'Core' 
        ELSE 'AddIn' 
    END AS "Type", "U_Installed" AS "Installed", 'S' AS "Status", '...' AS "History" 
FROM "@DOVER_MODULES"
ORDER BY "Code"
