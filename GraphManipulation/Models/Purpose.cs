using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Purpose : DomainEntity, IPurpose
{
    public bool LegallyRequired { get; set; }

    public virtual IEnumerable<DeleteCondition>? DeleteConditions { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; }
    public virtual IEnumerable<VacuumingRule>? Rules { get; set; }

    public string ToListing()
    {
        return string.Join(", ",
            Name,
            Description,
            LegallyRequired,
            "[ " + string.Join(", ",
                DeleteConditions is null ? new List<string>() : DeleteConditions.Select(r => r.ToListingIdentifier())) +
            " ]",
            "[ " + string.Join(", ", Rules is null ? new List<string>() : Rules.Select(r => r.ToListingIdentifier())) +
            " ]"
        );
    }

    public string ToListingIdentifier()
    {
        return GetName();
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

    public IEnumerable<string> GetDeleteCondition()
    {
        return DeleteConditions == null ? new List<string>() : DeleteConditions.Select(p => p.GetName());
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