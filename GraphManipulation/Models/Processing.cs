using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class Processing : DomainEntity, IProcessing
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Purpose Purpose { get; set; }
    public PersonalDataColumn PersonalDataColumn { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Purpose.ToListing(), PersonalDataColumn.ToListing());
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }

    public IPurpose GetPurpose()
    {
        return Purpose;
    }

    public IPersonalDataColumn GetPersonalDataColumn()
    {
        return PersonalDataColumn;
    }
}