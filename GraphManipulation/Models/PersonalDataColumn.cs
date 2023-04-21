using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class PersonalDataColumn : DomainEntity, IPersonalDataColumn
{
    public TableColumnPair TableColumnPair { get; set; }
    public string? Description { get; set; }
    public IEnumerable<Purpose> Purposes { get; set; }
    
    public string JoinCondition { get; set; }


    public string ToListing()
    {
        return string.Join(", ", TableColumnPair.ToListing(), Description,
            "[ " + Purposes.Select(p => p.ToListing()) + " ]");
    }

    public string GetDescription()
    {
        return Description ?? "";
    }

    public TableColumnPair GetTableColumnPair()
    {
        return TableColumnPair;
    }

    public IEnumerable<IPurpose> GetPurposes()
    {
        return Purposes;
    }
}