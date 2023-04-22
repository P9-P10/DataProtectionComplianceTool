using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Purpose : DomainEntity, IPurpose
{
    public bool LegallyRequired { get; set; }
    public IEnumerable<PersonalDataColumn> Columns { get; set; }
    public DeleteCondition? DeleteCondition { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; }
    public IEnumerable<VacuumingRule> Rules { get; set; }

    public string ToListing()
    {
        return string.Join(", ",
            Name,
            Description,
            LegallyRequired,
            "[ " + (DeleteCondition is null ? "" : DeleteCondition.ToListing()) + " ]",
            "[ " + string.Join(", ", Columns.Select(c => c.ToListing())) + " ]",
            "[ " + string.Join(", ",  Rules.Select(r => r.ToListing())) + " ]"
        );
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description ?? "";
    }

    public bool GetLegallyRequired()
    {
        return LegallyRequired;
    }

    public IDeleteCondition? GetDeleteCondition()
    {
        return DeleteCondition;
    }


    public override bool Equals(object? obj)
    {
        return Equals(obj as Purpose);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Id, Description);
    }

    bool Equals(Purpose? other)
    {
        return other.Description == Description && other.Name == Name && other.Id == Id;
    }
}