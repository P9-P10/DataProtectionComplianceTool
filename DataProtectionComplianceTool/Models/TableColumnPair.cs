namespace GraphManipulation.Models;

public class TableColumnPair
{
    public TableColumnPair(string tableName, string columnName)
    {
        TableName = tableName;
        ColumnName = columnName;
    }

    public string TableName { get; set; }
    public string ColumnName { get; set; }

    public override string ToString()
    {
        return "(" + string.Join(", ", TableName, ColumnName) + ")";
    }

    public override bool Equals(object? obj)
    {
        return obj is not null && Equals((obj as TableColumnPair)!);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TableName, ColumnName);
    }

    public bool Equals(TableColumnPair other)
    {
        return TableName == other.TableName && ColumnName == other.ColumnName;
    }
}