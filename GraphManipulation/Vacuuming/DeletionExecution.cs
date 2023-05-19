using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public class DeletionExecution
{
    public DeletionExecution(List<Purpose> purposes, string column, string table, string query,
        VacuumingRule vacuumingRule)
    {
        Purposes = purposes;
        Column = column;
        Table = table;
        Query = query;
        VacuumingRule = vacuumingRule;
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

    public VacuumingRule VacuumingRule { get; set; }

    public void SetTableAndColum(PersonalDataColumn personalDataColumn)
    {
        Column = personalDataColumn.Key.ColumnName;
        Table = personalDataColumn.Key.TableName;
    }

    public void CreateQuery(PersonalDataColumn personalDataColumn, IEnumerable<StorageRule> storageRules)
    {
        string updateQuery =
            $"UPDATE {personalDataColumn.Key.TableName} SET {personalDataColumn.Key.ColumnName} = '{personalDataColumn.DefaultValue}' WHERE ";

        List<string> conditions = storageRules.Select(rule => $"({rule.VacuumingCondition})").ToList();
        string combinedCondition = string.Join(" AND ", conditions) + ";";

        Query = updateQuery + combinedCondition;
    }

    public void SetPurposesFromRules(IEnumerable<StorageRule> storageRules)
    {
        Purposes = storageRules.SelectMany(rule => rule.Purposes).ToList();
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