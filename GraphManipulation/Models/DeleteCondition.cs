using GraphManipulation.Managers.Interfaces.Base;

namespace GraphManipulation.Models;

public class DeleteCondition : DomainEntity, IListable
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Condition { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Description, Condition);
    }
}