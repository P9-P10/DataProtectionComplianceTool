using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Managers;

public class TableColumnPair : IListable
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }

    public TableColumnPair(string tableName, string columnName)
    {
        TableName = tableName;
        ColumnName = columnName;
    }

    public string ToListing()
    {
        return "(" + string.Join(", ", TableName, ColumnName) + ")";
    }

    public string ToListingIdentifier()
    {
        return ToListing();
    }

    public override string ToString()
    {
        return ToListing();
    }

    public bool Equals(TableColumnPair other)
    {
        return TableName == other.TableName && ColumnName == other.ColumnName;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TableColumnPair);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TableName,ColumnName);
    }
}