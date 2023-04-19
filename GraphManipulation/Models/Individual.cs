using GraphManipulation.Managers.Interfaces.Base;

namespace GraphManipulation.Models;

public class Individual : DomainEntity, IListable
{
    public string ToListing()
    {
        return Id.ToString() ?? "Unknown";
    }
}