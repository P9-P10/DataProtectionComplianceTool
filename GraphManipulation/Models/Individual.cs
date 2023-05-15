using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class Individual : Entity<int>
{
    public virtual IEnumerable<PersonalDataOrigin>? PersonalDataOrigins { get; set; }

    public override string ToListing()
    {
        return string.Join(", ", base.ToListing(), 
            "[ " + 
            string.Join(", ", PersonalDataOrigins == null 
                ? new List<string>() 
                : PersonalDataOrigins.Select(pdo => pdo.ToListingIdentifier())) 
            + " ]");
    }
}