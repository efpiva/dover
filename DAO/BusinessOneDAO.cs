using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddOne.Framework.Model.SAP.Assembly;

namespace AddOne.Framework.DAO
{
    public interface BusinessOneDAO
    {

        void CreateTables(List<Model.SAP.GAUserTable> tables);

        void CreateTable(Model.SAP.GAUserTable table);

        void CreateField(Model.SAP.GAUserField field);

        List<Model.SAP.GAUserTable> CheckTablesExists(List<Model.SAP.GAUserTable> tables);

        List<Model.SAP.GAUserField> CheckFieldsExists(List<Model.SAP.GAUserTable> tables);

        void CreateFields(List<Model.SAP.GAUserField> fieldsToCreate);

        string GetNextCode(String udt);

        string GetCurrentUser();

        void ExecuteStatement(string sql);

        List<AssemblyInformation> GetCoreAssemblies();

        List<AssemblyInformation> GetAddinsAssemblies();

        byte[] GetAssembly(AssemblyInformation asm);
    }
}
