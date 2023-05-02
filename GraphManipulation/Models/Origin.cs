using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Origin : DomainEntity, IOrigin
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<PersonalDataColumn>? PersonalDataColumns { get; set; }

    public string ToListing()
    {
        return string.Join(", ", Name, Description,
            "[ " + string.Join(", ",
                PersonalDataColumns is null ? " " : PersonalDataColumns.Select(c => c.ToListing())) + " ]");
    }

    public IEnumerable<IPersonalDataColumn>? GetPersonalDataColumns()
    {
        return PersonalDataColumns;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }
}