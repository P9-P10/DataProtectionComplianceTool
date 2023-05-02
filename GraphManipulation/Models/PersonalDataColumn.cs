using GraphManipulation.Managers;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class PersonalDataColumn : DomainEntity, IPersonalDataColumn
{
    public TableColumnPair? TableColumnPair { get; set; }
    public string? Description { get; set; }
    public IEnumerable<Purpose>? Purposes { get; set; }

    public string DefaultValue { get; set; } = "";

    public string JoinCondition { get; set; } = "";


    public string ToListing()
    {
        return string.Join(", ", TableColumnPair == null ? " " : TableColumnPair.ToListing(), JoinCondition,
            Description, DefaultValue,
            "[ " + string.Join(", ", Purposes == null ? new List<string>() : Purposes.Select(p => p.ToListing())) + " ]");
    }

    public string GetDescription()
    {
        return Description ?? "";
    }

    public TableColumnPair GetTableColumnPair()
    {
        return TableColumnPair;
    }

    public void AddPurpose(Purpose purpose)
    {
        Purposes = Purposes == null ? new List<Purpose>() {purpose} : Purposes.Concat(new List<Purpose>() {purpose});
    }

    public IEnumerable<IPurpose> GetPurposes()
    {
        return Purposes == null ? new List<IPurpose>() : Purposes;
    }

    public string GetJoinCondition()
    {
        return JoinCondition;
    }

    public string GetDefaultValue()
    {
        return DefaultValue;
    }
}