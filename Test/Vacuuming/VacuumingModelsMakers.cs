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
            StoragePolicy storagePolicy = new()
            {
                Id = 0,
                Key = "Name",
                Description = "Description",
                VacuumingCondition = "Condition",
            };
            purposes.Add(new Purpose()
            {
                Id = 0, Key = purposeName, Description = "Description",
                StoragePolicies = new List<StoragePolicy>() {storagePolicy}
            });
        }
        else
        {
            StoragePolicy storagePolicy = new()
            {
                Id = 0,
                Key = "Name",
                Description = "Description",
                VacuumingCondition = "FirstCondition"
            };
            StoragePolicy deleteCondition2 = new()
            {
                Id = 1,
                Key = "SecondName",
                Description = "Description",
                VacuumingCondition = "SecondCondition"
            };
            purposes.Add(new Purpose()
                {Id = 0, Key = purposeName, StoragePolicies = new List<StoragePolicy>() {storagePolicy}});
            purposes.Add(new Purpose()
                {Id = 1, Key = purposeName, StoragePolicies = new List<StoragePolicy>() {deleteCondition2}});
        }

        PersonalDataColumn personalDataColumn = new()
        {
            Key = new TableColumnPair(tableName, columnName),
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
        DeletionExecution deletionExecution = new(purposes, column, table, query,new VacuumingPolicy());
        return deletionExecution;
    }

    public static VacuumingPolicy VacuumingPolicyMaker(string name = "Name", string description = "Description",
        string duration = "2y 5d", IEnumerable<Purpose>? purposes = null)
    {
        purposes ??= PurposesMaker();
        return new VacuumingPolicy(description, name, duration, purposes) {Id = 0};
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
                StoragePolicies = new List<StoragePolicy> {DeleteConditionMaker()},
                VacuumingPolicies = new List<VacuumingPolicy>()
            }
        };
    }

    public static Purpose PurposeMaker(string name = "Name", int id = 0, string tableName = "Table",
        string columnName = "Column",
        string defaultValue = "Null", string condition = "Condition")
    {
        StoragePolicy storagePolicy = DeleteConditionMaker(tableName, columnName, defaultValue, condition);
        List<PersonalDataColumn> personalDataColumns =
            PersonalDataColumns(tableName, columnName, defaultValue).ToList();

        Purpose purpose = new()
        {
            Key = name,
            StoragePolicies = new List<StoragePolicy>(),
            VacuumingPolicies = new List<VacuumingPolicy>(),
            Id = id
        };
        foreach (var column in personalDataColumns)
        {
            column.Purposes ??= new List<Purpose>();

            column.Purposes = column.Purposes.Append(purpose).ToList();
        }
        
        storagePolicy.Purposes = storagePolicy.Purposes.Append(purpose);
        purpose.StoragePolicies = purpose.StoragePolicies.Append(storagePolicy).ToList();
        return purpose;
    }

    public static StoragePolicy DeleteConditionMaker(string tableName = "Table", string columnName = "Column",
        string defaultValue = "Null", string condition = "Condition")
    {
        return new StoragePolicy()
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
                Key = new TableColumnPair(table, column)
            }
        };
    }
}