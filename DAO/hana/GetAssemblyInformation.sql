SELECT "Code", "U_Name" AS "Name", IFNULL("U_Description", "U_Name") AS "Description", "U_FileName" AS "FileName", 
    "U_Version" AS "Version", U_MD5 AS "MD5", "U_Date" AS "Date", "U_Size" AS "Size", "U_Type" AS "TypeCode",
	"U_Namespace" AS "Namespace"
FROM "@DOVER_MODULES" 
WHERE "Code" = '{0}' 