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

    public void AddPurpose(Purpose purpose)
    {
        if (!_purposes.Contains(purpose))
        {
            _purposes.Add(purpose);
        }
    }

    public IEnumerable<Purpose> GetPurposeWithLegalReason()
    {
        var purposes = new List<Purpose>();
        foreach (var purpose in _purposes)
        {
            if (purpose.GetLegallyRequired)
            {
                purposes.Add(purpose);
            }
        }

        return purposes;
    }

    public Purpose GetPurposeWithOldestExpirationDate()
    {
        return _purposes.Aggregate(((purpose, purpose1) =>
            purpose.GetExpirationDateAsDateTime < purpose1.GetExpirationDateAsDateTime ? purpose : purpose1));
    }
}