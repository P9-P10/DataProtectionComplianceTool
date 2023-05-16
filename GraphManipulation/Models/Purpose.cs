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
        return string.Join(", ", base.ToListing(),
            NullToString(LegallyRequired),
            ListNullOrEmptyToString(StorageRules),
            ListNullOrEmptyToString(PersonalDataColumns),
            ListNullOrEmptyToString(Rules));
    }
}