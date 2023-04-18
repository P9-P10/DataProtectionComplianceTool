using System.Collections.Generic;
using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace Test.Vacuuming;

public static class VacuumingModelsMakers
{
    public static PersonDataColumn PersonDataColumnMaker(string defaultValue = "Null",
        bool multipleDeleteConditions = false,
        string tableName = "Table", string columnName = "Column", string purpose = "Purpose")
    {
        List<DeleteCondition> deleteConditions = new List<DeleteCondition>();
        if (!multipleDeleteConditions)
        {
            DeleteCondition deleteCondition = new("Condition", new Purpose
            {
                Id = 0,
                Name = "Name",
                Description = "Description"
                    
            });
            deleteConditions.Add(deleteCondition);
        }
        else
        {
            DeleteCondition deleteCondition = new("Condition", new Purpose
            {
                Id = 0,
                Name = "Name",
                Description = "Description"
                    
            });
            DeleteCondition deleteCondition2 = new("SecondCondition", new Purpose
            {
                Id = 1,
                Name = "SecondName",
                Description = "Description"
                    
            });
            deleteConditions.Add(deleteCondition);
            deleteConditions.Add(deleteCondition2);
        }

        PersonDataColumn personDataColumn = new(tableName,
            columnName,
            defaultValue,
            deleteConditions);
        return personDataColumn;
    }

    public static DeletionExecution DeletionExecutionMaker(string query, string table = "Table",
        string column = "Column")
    {
        List<Purpose> purposes = new List<Purpose>() {new()
        {
            Name = "Name",
            Description = "Description"
        }};
        DeletionExecution deletionExecution = new(purposes, column, table, query);
        return deletionExecution;
    }

    public static VacuumingRule VacuumingRuleMaker(string name = "Name", string description = "Description",
        string interval = "2y 5d", IEnumerable<Purpose>? purposes = null)
    {
        purposes ??= PurposesMaker();
        return new VacuumingRule(name, description, interval, purposes);
    }

    private static IEnumerable<Purpose> PurposesMaker()
    {
        List<Purpose> purposes = new List<Purpose>();

        purposes.Add(new Purpose()
        {
           Id = 0, 
           Name ="Name",
           Description = "Description",
           Columns = PersonDataColumns(),
           Rules = new List<VacuumingRule>()
        });

        return purposes;
    }

    private static IEnumerable<Column>? PersonDataColumns()
    {
        return new List<Column>() {new()
        {
           Id = 0,
           TableName = "Table",
           ColumnName = "Column",
           Purposes = new List<Purpose>()
        }};
    }
}