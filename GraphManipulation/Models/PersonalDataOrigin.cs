using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class PersonalDataOrigin : Entity<int>
{
    public virtual Individual? Individual { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public virtual Origin? Origin { get; set; }

    public override string ToListing()
    {
        return base.ToListing() + "(" + string.Join(ToListingSeparator,
            NullToString(Individual),
            NullToString(PersonalDataColumn),
            NullToString(Origin)) + ")";
    }
    
    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Individual", "Personal Data Column", "Origin");
    }
}