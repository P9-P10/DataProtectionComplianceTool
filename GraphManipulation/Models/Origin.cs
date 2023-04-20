
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class Origin : DomainEntity, IOrigin
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<IPersonalDataColumn> PersonalDataColumns { get; set; }
    
    public string ToListing()
    {
        return string.Join(", ", Name, Description, "[ " + PersonalDataColumns.Select(c => c.ToListing()) + " ]");
    }
}