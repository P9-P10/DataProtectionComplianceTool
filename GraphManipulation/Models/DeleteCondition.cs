namespace GraphManipulation.Models;

public class DeleteCondition : DomainEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Condition { get; set; }
}