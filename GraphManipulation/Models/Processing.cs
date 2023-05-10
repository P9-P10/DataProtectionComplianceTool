using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class Processing : DomainEntity, IProcessing
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual Purpose Purpose { get; set; }
    public virtual PersonalDataColumn PersonalDataColumn { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Description, Purpose.ToListingIdentifier(), PersonalDataColumn.TableColumnPair.ToListingIdentifier());
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
        return Description;
    }

    public IPurpose GetPurpose()
    {
        return Purpose;
    }

    public TableColumnPair GetPersonalDataTableColumnPair()
    {
        return PersonalDataColumn.TableColumnPair;
    }
}