using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Purpose : Entity<string>
{
    public bool LegallyRequired { get; set; }
    public virtual IEnumerable<PersonalDataColumn>? PersonalDataColumns { get; set; }
    public virtual IEnumerable<DeleteCondition>? DeleteConditions { get; set; }
    public virtual IEnumerable<VacuumingRule>? Rules { get; set; }

    public override string ToListing()
    {
        return string.Join(", ", base.ToListing(), LegallyRequired,
            "[ " + string.Join(", ", DeleteConditions is null ? new List<string>() : DeleteConditions.Select(c => c.ToListingIdentifier())) + " ]",
            "[ " + string.Join(", ", PersonalDataColumns is null ? new List<string>() : PersonalDataColumns.Select(c => c.ToListingIdentifier())) + " ]",
            "[ " + string.Join(", ", Rules is null ? new List<string>() : Rules.Select(r => r.ToListingIdentifier())) + " ]"
        );
    }
    
    // public override void Fill(object? other)
    // {
    //     if (other is null || other.GetType() != typeof(Purpose))
    //     {
    //         return;
    //     }
    //     
    //     base.Fill(other);
    //
    //     var otherPurpose = (other as Purpose)!;
    //     
    //     otherPurpose.PersonalDataColumns ??= PersonalDataColumns;
    //     otherPurpose.DeleteConditions ??= DeleteConditions;
    //     otherPurpose.Rules ??= Rules;
    // }
}