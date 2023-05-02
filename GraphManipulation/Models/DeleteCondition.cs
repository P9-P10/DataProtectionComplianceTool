using GraphManipulation.Models.Interfaces;

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

    public string GetCondition()
    {
        return Condition;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }
}