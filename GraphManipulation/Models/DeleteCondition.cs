using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class DeleteCondition : Entity<string>
{
    // TODO Eventuelt omdøb Condition til VacuumingCondition
    public string? Condition { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public virtual IEnumerable<Purpose>? Purposes { get; set; }
    
    public override string ToListing()
    {
        return string.Join(", ", base.ToListing(), Condition, PersonalDataColumn?.ToListingIdentifier(), 
            "[ " + string.Join(", ", Purposes == null ? new List<string>() : Purposes.Select(p => p.ToListingIdentifier())) + " ]");
    }
}