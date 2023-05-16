using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class StorageRule : Entity<string>
{
    public string? VacuumingCondition { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public virtual IEnumerable<Purpose>? Purposes { get; set; }
    
    public override string ToListing()
    {
        return string.Join(", ", base.ToListing(),
            NullToString(VacuumingCondition),
            NullToString(PersonalDataColumn),
            ListNullOrEmptyToString(Purposes));
    }
}