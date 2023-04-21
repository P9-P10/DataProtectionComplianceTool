using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class Individual : DomainEntity, IIndividual
{
    public string ToListing()
    {
        return Id == null ? "Unknown" : Id.ToString();
    }
}