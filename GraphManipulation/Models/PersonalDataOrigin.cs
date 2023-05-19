using GraphManipulation.Commands;

namespace GraphManipulation.Models;

public class PersonalDataOrigin : Entity<int>
{
    public virtual Individual? Individual { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public virtual Origin? Origin { get; set; }

    public override string ToListing()
    {
        return ToListingIdentifier() + ToListingSeparator + "(" + string.Join(ToListingSeparator,
            NullToString(Individual),
            NullToString(PersonalDataColumn),
            NullToString(Origin)) + ")";
    }
    
    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, "Key", "Individual", "Personal Data Column", "Origin");
    }
}