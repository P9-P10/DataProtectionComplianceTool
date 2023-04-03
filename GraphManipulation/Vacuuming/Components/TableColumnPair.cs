﻿namespace GraphManipulation.Vacuuming.Components;

public class TableColumnPair
{
    private string _table;
    private string _column;
    private string _updateValue;
    private List<Purpose> _purposes = new();
    public string Table => _table;
    public string Column => _column;
    public string UpdateValue => _updateValue;

    public List<Purpose> GetPurposes => _purposes;

    public TableColumnPair(string table, string column,string updateValue = "Null")
    {
        _table = table;
        _column = column;
        _updateValue = updateValue;
    }

    /// <summary>
    /// Constructor used to create TableColumn pair from dapper.
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
        _table = target_table;
        _column = target_column;
        _updateValue = "Null";

        Purpose newPurpose = new(purpose, ttl, start_time, origin, legally_required.ToLower() == "1");
        AddPurpose(newPurpose);
    }

    public void AddPurpose(Purpose purpose)
    {
        if (!_purposes.Contains(purpose))
        {
            _purposes.Add(purpose);
        }
    }

    public void AddPurposes(List<Purpose> purposes)
    {
        foreach (Purpose purpose in purposes)
        {
            AddPurpose(purpose);
        }
    }

    public IEnumerable<Purpose> GetPurposeWithLegalReason()
    {
        return _purposes.Where(purpose => purpose.GetLegallyRequired).ToList();
    }

    public Purpose GetPurposeWithOldestExpirationDate()
    {
        return _purposes.Aggregate(((purpose, purpose1) =>
            purpose.GetExpirationDateAsDateTime < purpose1.GetExpirationDateAsDateTime ? purpose : purpose1));
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TableColumnPair);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_table, _column);
    }

    private bool Equals(TableColumnPair? other)
    {
        return other != null && Table == other.Table && Column == other.Column;
    }
    
}