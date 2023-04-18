
namespace GraphManipulation.Models;

public class Origin : DomainEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<PersonalDataColumn> Columns { get; set; }
}