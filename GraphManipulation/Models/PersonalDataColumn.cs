using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class PersonalDataColumn : DomainEntity, IPersonalDataColumn
{
    public TableColumnPair TableColumnPair { get; set; }
    public string Description { get; set; }
    public IEnumerable<IPurpose> Purposes { get; set; }


    public string ToListing()
    {
        return string.Join(", ", TableColumnPair.ToListing(), Description,
            "[ " + Purposes.Select(p => p.ToListing()) + " ]");
    }
}