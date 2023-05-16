namespace GraphManipulation.Models;

public class Origin : Entity<string>
{
    public virtual IEnumerable<PersonalDataColumn>? PersonalDataColumns { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(), 
            ListNullOrEmptyToString(PersonalDataColumns));
    }

    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Personal Data Columns");
    }
}