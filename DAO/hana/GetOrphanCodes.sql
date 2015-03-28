select Code from "@DOVER_MODULES" where "U_Type" = 'D' and "Code" not in (select "U_DepCode" from "@DOVER_MODULES_DEP")
