namespace GraphManipulation.DataAccess.Entities;

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