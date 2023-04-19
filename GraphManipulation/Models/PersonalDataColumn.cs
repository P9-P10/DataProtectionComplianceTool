using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces.Base;

namespace GraphManipulation.Models;

public class PersonalDataColumn : DomainEntity, IListable
{
    public TableColumnPair TableColumnPair { get; set; }
    public string? Description { get; set; }
    public IEnumerable<Purpose> Purposes { get; set; }


    public string ToListing()
    {
        return string.Join(", ", TableColumnPair.ToListing(), Description,
            "[ " + Purposes.Select(p => p.ToListing()) + " ]");
    }
}