using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class PersonalDataOrigin : Entity<int>
{
    public virtual Individual? Individual { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public virtual Origin? Origin { get; set; }

    public override string ToListing()
    {
        return base.ToListing() + "(" + string.Join(", ", 
            Individual?.ToListingIdentifier(), 
            PersonalDataColumn?.ToListingIdentifier(),
            Origin?.ToListingIdentifier()) + ")";
    }
}