
using GraphManipulation.Managers.Interfaces.Base;

namespace GraphManipulation.Models;

public class Origin : DomainEntity, IListable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<PersonalDataColumn> Columns { get; set; }
    
    public string ToListing()
    {
        return string.Join(", ", Name, Description, "[ " + Columns.Select(c => c.ToListing()) + " ]");
    }
}