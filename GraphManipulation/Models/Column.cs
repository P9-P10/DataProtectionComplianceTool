namespace GraphManipulation.Models;

public class Column : DomainEntity
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public IEnumerable<Purpose> Purposes { get; set; }
}