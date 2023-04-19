using GraphManipulation.Managers.Interfaces.Base;

namespace GraphManipulation.Models;

public class Processing : DomainEntity, IListable
{
    public string Name { get; set; }
    public Purpose Purpose { get; set; }
    public PersonalDataColumn PersonalDataColumn { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Purpose.ToListing(), PersonalDataColumn.ToListing());
    }
}