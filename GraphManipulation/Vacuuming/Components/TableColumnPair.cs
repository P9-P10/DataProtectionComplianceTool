namespace GraphManipulation.Vacuuming.Components;

public class TableColumnPair
{
    private string _table;
    private string _column;
    private List<Purpose> _purposes = new();
    public string Table => _table;
    public string Column => _column;


    public List<Purpose> GetPurposes => _purposes;

    public TableColumnPair(string table, string column)
    {
        _table = table;
        _column = column;
    }

    public TableColumnPair(string target_table, string target_column, string purpose, string ttl,
        bool legally_required, string origin, string start_time)
    {
        _table = target_table;
        _column = target_column;
        Purpose newPurpose = new(purpose, ttl, start_time, origin, legally_required);
        AddPurpose(newPurpose);
    }

    public void AddPurpose(Purpose purpose)
    {
        if (!_purposes.Contains(purpose))
        {
            _purposes.Add(purpose);
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
}