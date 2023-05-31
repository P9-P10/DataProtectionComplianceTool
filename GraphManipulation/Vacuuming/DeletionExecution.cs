using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public class DeletionExecution
{
    public DeletionExecution(List<Purpose> purposes, string column, string table, string query,
        VacuumingPolicy vacuumingPolicy)
    {
        Purposes = purposes;
        Column = column;
        Table = table;
        Query = query;
        VacuumingPolicy = vacuumingPolicy;
    }

    public DeletionExecution()
    {
        Purposes = new List<Purpose>();
        Column = "";
        Query = "";
        Table = "";
    }

    public string Table { get; set; }

    public string Column { get; set; }

    public string Query { get; set; }

    public List<Purpose> Purposes { get; set; }

    public VacuumingPolicy VacuumingPolicy { get; set; }

    public void SetTableAndColum(PersonalDataColumn personalDataColumn)
    {
        Column = personalDataColumn.Key.ColumnName;
        Table = personalDataColumn.Key.TableName;
    }

    public void CreateQuery(PersonalDataColumn personalDataColumn, IEnumerable<StoragePolicy> storagePolicies)
    {
        string updateQuery =
            $"UPDATE {personalDataColumn.Key.TableName} SET {personalDataColumn.Key.ColumnName} = '{personalDataColumn.DefaultValue}' WHERE ";

        List<string> conditions = storagePolicies.Select(storagePolicy => $"({storagePolicy.VacuumingCondition})").ToList();
        string combinedCondition = string.Join(" AND ", conditions) + ";";

        Query = updateQuery + combinedCondition;
    }

    public void SetPurposesFromStoragePolicies(IEnumerable<StoragePolicy> storagePolicies)
    {
        Purposes = storagePolicies.SelectMany(storagePolicy => storagePolicy.Purposes).ToList();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DeletionExecution);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Table, Column, Query);
    }

    public bool Equals(DeletionExecution? other)
    {
        return other.Column == Column && other.Table == Table && other.Query == Query;
    }
}