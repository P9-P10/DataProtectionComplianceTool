using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public class DeletionExecution
{
    public string Table { get; set; }

    public string Column { get; set; }

    public string Query { get; set; }

    public List<Purpose> Purposes { get; set; }

    public DeletionExecution(List<Purpose> purposes, string column, string table, string query)
    {
        Purposes = purposes;
        Column = column;
        Table = table;
        Query = query;
    }

    public DeletionExecution()
    {
        Purposes = new List<Purpose>();
        Column = "";
        Query = "";
        Table = "";
    }

    public void AddPurpose(Purpose purpose)
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