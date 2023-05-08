using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Individual : DomainEntity, IIndividual
{
    
    public virtual IEnumerable<PersonalData>? PersonalData { get; set; }
    public string ToListing()
    {
        return Id == null ? "Unknown" : Id.ToString();
    }

    public string ToListingIdentifier()
    {
        return ToListing();
    }
}