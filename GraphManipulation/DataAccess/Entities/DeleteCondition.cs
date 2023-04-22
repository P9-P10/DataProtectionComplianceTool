
using GraphManipulation.Models;

namespace GraphManipulation.DataAccess.Entities;

// TODO: Consider moving to another directory, as this is not mapped directly to the database
public class DeleteCondition
{
    public DeleteCondition(string condition, Purpose purpose)
    {
        Condition = condition;
        Purpose = purpose;
    }

    public string Condition { get; set; }
    public Purpose Purpose { get; set; }
}