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
            StorageRule storageRule = new()
            {
                Id = 0,
                Key = "Name",
                Description = "Description",
                VacuumingCondition = "Condition",
            };
            purposes.Add(new Purpose()
            {
                Id = 0, Key = purposeName, Description = "Description",
                DeleteConditions = new List<StorageRule>() {storageRule}
            });
        }
        else
        {
            StorageRule storageRule = new()
            {
                Id = 0,
                Key = "Name",
                Description = "Description",
                VacuumingCondition = "FirstCondition"
            };
            StorageRule deleteCondition2 = new()
            {
                Id = 1,
                Key = "SecondName",
                Description = "Description",
                VacuumingCondition = "SecondCondition"
            };
            purposes.Add(new Purpose()
                {Id = 0, Key = purposeName, DeleteConditions = new List<StorageRule>() {storageRule}});
            purposes.Add(new Purpose()
                {Id = 1, Key = purposeName, DeleteConditions = new List<StorageRule>() {deleteCondition2}});
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
                Key = "Name",
                Description = "Description"
            }
        };
        DeletionExecution deletionExecution = new(purposes, column, table, query,VacuumingRuleMaker());
        return deletionExecution;
    }

    public static VacuumingRule VacuumingRuleMaker(string Key = "Name", string description = "Description",
        string interval = "2y 5d", IEnumerable<Purpose>? purposes = null)
    {
        purposes ??= PurposesMaker();
        return new VacuumingRule(description, Key, interval, purposes) {Id = 0};
    }

    private static IEnumerable<Purpose> PurposesMaker()
    {
        return new List<Purpose>
        {
            new()
            {
                Id = 0,
                Key = "Name",
                Description = "Description",
                DeleteConditions = new List<StorageRule> {DeleteConditionMaker()},
                Rules = new List<VacuumingRule>()
            }
        };
    }

    public static Purpose PurposeMaker(string name = "Name", int id = 0, string tableName = "Table",
        string columnName = "Column",
        string defaultValue = "Null", string condition = "Condition")
    {
        StorageRule storageRule = DeleteConditionMaker(tableName, columnName, defaultValue, condition);
        List<PersonalDataColumn> personalDataColumns =
            PersonalDataColumns(tableName, columnName, defaultValue).ToList();

        Purpose purpose = new()
        {
            Key = name,
            DeleteConditions = new List<StorageRule>(),
            Rules = new List<VacuumingRule>(),
            Id = id
        };
        foreach (var column in personalDataColumns)
        {
            column.Purposes ??= new List<Purpose>();

            column.Purposes = column.Purposes.Append(purpose).ToList();
        }
        
        storageRule.Purposes = storageRule.Purposes.Append(purpose);
        purpose.DeleteConditions = purpose.DeleteConditions.Append(storageRule).ToList();
        return purpose;
    }

    public static StorageRule DeleteConditionMaker(string tableName = "Table", string columnName = "Column",
        string defaultValue = "Null", string condition = "Condition")
    {
        return new StorageRule()
        {
            Key = "Execution",
            VacuumingCondition = condition,
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