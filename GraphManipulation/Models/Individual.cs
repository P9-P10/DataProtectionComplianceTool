using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Individual : DomainEntity, IIndividual
{
    
    public IEnumerable<PersonalData> PersonalData { get; set; }
    public string ToListing()
    {
        return Id == null ? "Unknown" : Id.ToString();
    }
}