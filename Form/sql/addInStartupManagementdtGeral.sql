Select T0.U_Name As Name, T0.U_Description As Description, T0.U_Version As Version, 
    IsNull(T0.U_Status,'A') As Status, Code From [@DOVER_MODULES] T0 WHERE U_Type = 'A'