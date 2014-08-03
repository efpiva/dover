SELECT "TableID" AS "Id1", CAST("FieldID" AS nvarchar) AS "Id2", '[' || "TableID" || '].' || "AliasID" AS "Code", 
    "Descr" AS "Name" 
FROM CUFD