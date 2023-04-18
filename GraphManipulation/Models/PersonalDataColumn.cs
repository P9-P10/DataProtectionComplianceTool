namespace GraphManipulation.Models;

public class PersonalDataColumn : DomainEntity
{
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public string Description { get; set; }
    public IEnumerable<Purpose> Purposes { get; set; }
}