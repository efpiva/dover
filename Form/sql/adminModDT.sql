select U_Name Name, U_Description Description, U_Version Version, case when U_Type = 'C' THEN 'Core' else 'AddIn' End Type, U_Installed Installed, 'S' Status, '...' History from [@DOVER_MODULES] 
WHERE U_Type in ('C', 'A')
order by Code