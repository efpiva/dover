SELECT "TableID" AS "Id1", CAST("FieldID" AS nvarchar) AS "Id2", cast('[' || "TableID" || '].' || "AliasID" AS nvarchar(20)) AS "Code", 
    cast("Descr" as nvarchar(100)) AS "Name" 
FROM CUFD