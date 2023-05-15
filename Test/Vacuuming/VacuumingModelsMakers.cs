using System.Collections.Generic;
using System.Linq;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace Test.Vacuuming;

public static class VacuumingModelsMakers
{
    public static PersonalDataColumn PersonalDataColumnMaker(string defaultValue = "Null",
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
                Condition = "Condition",
            };
            purposes.Add(new Purpose()
            {
                Id = 0, Name = purposeName, Description = "Description",
                DeleteConditions = new List<DeleteCondition>() {deleteCondition}
            });
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
            purposes.Add(new Purpose()
                {Id = 0, Name = purposeName, DeleteConditions = new List<DeleteCondition>() {deleteCondition}});
            purposes.Add(new Purpose()
                {Id = 1, Name = purposeName, DeleteConditions = new List<DeleteCondition>() {deleteCondition2}});
        }

        PersonalDataColumn personalDataColumn = new()
        {
            TableColumnPair = new TableColumnPair(tableName, columnName),
            DefaultValue = defaultValue,
            Purposes = purposes
        };
        return personalDataColumn;
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
        return new List<Purpose>
        {
            new()
            {
                Id = 0,
                Name = "Name",
                Description = "Description",
                DeleteConditions = new List<DeleteCondition> {DeleteConditionMaker()},
                Rules = new List<VacuumingRule>()
            }
        };
    }

    public static Purpose PurposeMaker(string name = "Name", int id = 0, string tableName = "Table",
        string columnName = "Column",
        string defaultValue = "Null", string condition = "Condition")
    {
        DeleteCondition deleteCondition = DeleteConditionMaker(tableName, columnName, defaultValue, condition);
        List<PersonalDataColumn> personalDataColumns =
            PersonalDataColumns(tableName, columnName, defaultValue).ToList();

        Purpose purpose = new()
        {
            Name = name,
            DeleteConditions = new List<DeleteCondition>(),
            Rules = new List<VacuumingRule>(),
            Id = id
        };
        foreach (var column in personalDataColumns)
        {
            column.Purposes ??= new List<Purpose>();

            column.Purposes = column.Purposes.Append(purpose).ToList();
        }
        
        deleteCondition.Purposes = deleteCondition.Purposes.Append(purpose);
        purpose.DeleteConditions = purpose.DeleteConditions.Append(deleteCondition).ToList();
        return purpose;
    }

    public static DeleteCondition DeleteConditionMaker(string tableName = "Table", string columnName = "Column",
        string defaultValue = "Null", string condition = "Condition")
    {
        return new DeleteCondition()
        {
            Name = "Execution",
            Condition = condition,
            Purposes = new List<Purpose>(),
            PersonalDataColumn =
                PersonalDataColumnMaker(tableName: tableName, columnName: columnName, defaultValue: defaultValue)
        };
    }

    private static IEnumerable<PersonalDataColumn> PersonalDataColumns(string table = "Table", string column = "Column",
        string defaultValue = "Null")
    {
        return new List<PersonalDataColumn>()
        {
            new()
            {
                Id = 0,
                Description = "Description",
                DefaultValue = defaultValue,
                Purposes = new List<Purpose>(),
                TableColumnPair = new TableColumnPair(table, column)
            }
        };
    }
}