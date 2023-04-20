using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class VacuumingRule : DomainEntity, IVacuumingRule
{
    public IEnumerable<Purpose> Purposes { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Interval { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Description, Interval, "[ " + string.Join(", ", Purposes.Select(p => p.ToListing())) + " ]");
    }
}