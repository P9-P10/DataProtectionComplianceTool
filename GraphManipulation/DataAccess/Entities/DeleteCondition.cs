
namespace GraphManipulation.DataAccess.Entities;

// TODO: Consider moving to another directory, as this is not mapped directly to the database
public class DeleteCondition
{
    public DeleteCondition(string condition, string purpose)
    {
        Condition = condition;
        Purpose = purpose;
    }

    public string Condition { get; set; }
    public string Purpose { get; set; }
}