using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class Individual : Entity<int>
{
    public virtual IEnumerable<PersonalDataOrigin>? PersonalDataOrigins { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, ToListingIdentifier(),
            ListNullOrEmptyToString(PersonalDataOrigins,
                origins => EncapsulateList(origins.Select(o => o.ToListing()))));
    }
    
    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Personal Data Origins");
    }
}