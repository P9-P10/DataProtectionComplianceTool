using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Individual : Entity<int>
{
    
    public virtual IEnumerable<PersonalData>? PersonalData { get; set; }
    
    public new string ToListing()
    {
        return Id == null ? "Unknown" : Id.ToString();
    }

    public new string ToListingIdentifier()
    {
        return ToListing();
    }
}