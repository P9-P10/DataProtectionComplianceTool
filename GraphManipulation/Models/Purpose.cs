using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class Purpose : Entity<string>
{
    public bool? LegallyRequired { get; set; }
    public virtual IEnumerable<PersonalDataColumn>? PersonalDataColumns { get; set; }
    public virtual IEnumerable<StorageRule>? StorageRules { get; set; }
    public virtual IEnumerable<VacuumingRule>? Rules { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(),
            NullToString(LegallyRequired),
            ListNullOrEmptyToString(StorageRules),
            ListNullOrEmptyToString(PersonalDataColumns),
            ListNullOrEmptyToString(Rules));
    }
    
    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Legally Required", "Storage Rules", "Personal Data Columns", "Vacuuming Rules");
    }
}