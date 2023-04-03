namespace GraphManipulation.Vacuuming.Components;

public class TableColumnPair
{
    public TableColumnPair(string table, string column, string updateValue = "Null")
    {
        Table = table;
        Column = column;
        UpdateValue = updateValue;
    }

    /// <summary>
    ///     Constructor used to create TableColumn pair from dapper.
    /// </summary>
    /// <param name="purpose"></param>
    /// <param name="target_table"></param>
    /// <param name="target_column"></param>
    /// <param name="ttl"></param>
    /// <param name="legally_required"></param>
    /// <param name="origin"></param>
    /// <param name="start_time"></param>
    public TableColumnPair(string purpose, string ttl, string target_table, string target_column,
        string legally_required, string origin, string start_time)
    {
        Table = target_table;
        Column = target_column;
        UpdateValue = "Null";

        Purpose newPurpose = new(purpose, ttl, start_time, origin, legally_required.ToLower() == "1");
        AddPurpose(newPurpose);
    }

    public string Table { get; }

    public string Column { get; }

    public string UpdateValue { get; }

    public List<Purpose> GetPurposes { get; } = new();

    public void AddPurpose(Purpose purpose)
    {
        if (!GetPurposes.Contains(purpose))
        {
            GetPurposes.Add(purpose);
        }
    }

    public void AddPurposes(List<Purpose> purposes)
    {
        foreach (var purpose in purposes)
            AddPurpose(purpose);
    }

    public IEnumerable<Purpose> GetPurposeWithLegalReason()
    {
        return GetPurposes.Where(purpose => purpose.GetLegallyRequired).ToList();
    }

    public Purpose GetPurposeWithOldestExpirationDate()
    {
        return GetPurposes.Aggregate((purpose, purpose1) =>
            purpose.GetExpirationDateAsDateTime < purpose1.GetExpirationDateAsDateTime ? purpose : purpose1);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TableColumnPair);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Table, Column);
    }

    private bool Equals(TableColumnPair? other)
    {
        return other != null && Table == other.Table && Column == other.Column;
    }
}