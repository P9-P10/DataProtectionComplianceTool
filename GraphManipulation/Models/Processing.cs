using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class Processing : Entity<string>
{
    public virtual Purpose? Purpose { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(), 
            NullToString(Purpose),
            NullToString(PersonalDataColumn));
    }
    
    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Purpose", "Personal Data Column");
    }
}