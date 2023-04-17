namespace GraphManipulation.Models;

public class Column
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public IEnumerable<Purpose> Purposes { get; set; }
}