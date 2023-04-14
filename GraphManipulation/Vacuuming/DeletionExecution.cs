namespace GraphManipulation.Vacuuming;

public class DeletionExecution
{
    public string Table { get; set; }

    public string Column { get; set; }

    public string Query { get; set; }

    public List<string> Purposes { get; set; }

    public DeletionExecution(List<string> purposes, string column, string table, string query)
    {
        Purposes = purposes;
        Column = column;
        Table = table;
        Query = query;
    }

    public DeletionExecution()
    {
        Purposes = new List<string>();
        Column = "";
        Query = "";
        Table = "";
    }

    public void AddPurpose(string purpose)
    {
        if (!Purposes.Contains(purpose))
        {
            Purposes.Add(purpose);
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DeletionExecution);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Table, Column, Query);
    }

    bool Equals(DeletionExecution? other)
    {
        return other.Column == Column && other.Table == Table && other.Query == Query;
    }
}