using System.Collections.Generic;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace Test.Vacuuming;

public static class VacuumingModelsMakers
{
    public static PersonalDataColumn PersonDataColumnMaker(string defaultValue = "Null",
        bool multipleDeleteConditions = false,
        string tableName = "Table", string columnName = "Column", string purposeName = "Purpose")
    {
        List<Purpose> purposes = new List<Purpose>();
        if (!multipleDeleteConditions)
        {
            DeleteCondition deleteCondition = new()
            {
                Id = 0,
                Name = "Name",
                Description = "Description",
                Condition = "Condition"
            };
            purposes.Add(new Purpose()
                {Id = 0, Name = purposeName, Description = "Description", DeleteCondition = deleteCondition});
        }
        else
        {
            DeleteCondition deleteCondition = new()
            {
                Id = 0,
                Name = "Name",
                Description = "Description",
                Condition = "FirstCondition"
            };
            DeleteCondition deleteCondition2 = new()
            {
                Id = 1,
                Name = "SecondName",
                Description = "Description",
                Condition = "SecondCondition"
            };
            purposes.Add(new Purpose() {Id = 0, Name = purposeName, DeleteCondition = deleteCondition});
            purposes.Add(new Purpose() {Id = 1, Name = purposeName, DeleteCondition = deleteCondition2});
        }

        PersonalDataColumn personDataColumn = new()
        {
            TableColumnPair = new TableColumnPair(tableName, columnName),
            DefaultValue = defaultValue,
            Purposes = purposes
        };
        return personDataColumn;
    }

    public static DeletionExecution DeletionExecutionMaker(string query, string table = "Table",
        string column = "Column")
    {
        List<Purpose> purposes = new List<Purpose>()
        {
            new()
            {
                Name = "Name",
                Description = "Description"
            }
        };
        DeletionExecution deletionExecution = new(purposes, column, table, query);
        return deletionExecution;
    }

    public static VacuumingRule VacuumingRuleMaker(string name = "Name", string description = "Description",
        string interval = "2y 5d", IEnumerable<Purpose>? purposes = null)
    {
        purposes ??= PurposesMaker();
        return new VacuumingRule(description, name, interval, purposes) {Id = 0};
    }

    private static IEnumerable<Purpose> PurposesMaker()
    {
        List<Purpose> purposes = new List<Purpose>();

        purposes.Add(new Purpose()
        {
            Id = 0,
            Name = "Name",
            Description = "Description",
            Columns = PersonDataColumns(),
            DeleteCondition = new DeleteCondition(),
            Rules = new List<VacuumingRule>()
        });

        return purposes;
    }

    private static IEnumerable<PersonalDataColumn> PersonDataColumns()
    {
        return new List<PersonalDataColumn>()
        {
            new()
            {
                Id = 0,
                Description = "Description",
                Purposes = new List<Purpose>(),
                TableColumnPair = new TableColumnPair("Table", "Column")
            }
        };
    }
}