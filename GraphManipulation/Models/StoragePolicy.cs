namespace GraphManipulation.Models;

public class StoragePolicy : Entity<string>
{
    public string? VacuumingCondition { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public virtual IEnumerable<Purpose>? Purposes { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(),
            NullToString(VacuumingCondition),
            NullToString(PersonalDataColumn),
            ListNullOrEmptyToString(Purposes));
    }

    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Vacuuming Condition", "Personal Data Column",
            "Purposes");
    }
}