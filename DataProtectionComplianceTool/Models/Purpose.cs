namespace GraphManipulation.Models;

public class Purpose : Entity<string>
{
    public bool? LegallyRequired { get; set; }
    public virtual IEnumerable<PersonalDataColumn>? PersonalDataColumns { get; set; }
    public virtual IEnumerable<StoragePolicy>? StoragePolicies { get; set; }
    public virtual IEnumerable<VacuumingPolicy>? VacuumingPolicies { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(),
            NullToString(LegallyRequired),
            ListNullOrEmptyToString(StoragePolicies),
            ListNullOrEmptyToString(PersonalDataColumns),
            ListNullOrEmptyToString(VacuumingPolicies));
    }

    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Legally Required", "Storage Policies",
            "Personal Data Columns", "Vacuuming Policies");
    }
}