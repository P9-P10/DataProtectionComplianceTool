namespace GraphManipulation.Models;

public class IndividualsSource
{
    public string Table { get; set; }
    public string Column { get; set; }

    public IndividualsSource(string table, string column)
    {
        Table = table;
        Column = column;
    }
}