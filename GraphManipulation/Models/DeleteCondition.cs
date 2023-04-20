using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class DeleteCondition : DomainEntity, IDeleteCondition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Condition { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Description, Condition);
    }
}