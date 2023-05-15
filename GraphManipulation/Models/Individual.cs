using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Individual : Entity<int>
{
    public virtual IEnumerable<PersonalDataOrigin>? PersonalDataOrigins { get; set; }

    public override string ToListing()
    {
        return string.Join(", ", Id is null ? "Unknown" : Id.ToString(), 
            "[ " + 
            string.Join(", ", PersonalDataOrigins is null 
                ? new List<PersonalDataOrigin>() 
                : PersonalDataOrigins.Select(pdo => pdo.ToListingIdentifier())) 
            + " ]");
    }
}