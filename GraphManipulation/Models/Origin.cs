
namespace GraphManipulation.Models;

public class Origin : DomainEntity
{
    public string Name { get; set; }
    public IEnumerable<Column> Columns { get; set; }
}