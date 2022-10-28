namespace GraphManipulation.Models;

public class Violation
{
    public Violation(Entity entity, string message)
    {
        Entity = entity;
        Message = message;
    }

    public Violation(Entity entity, string message, object obj)
    {
        Entity = entity;
        Message = message;
        Value = obj;
    }

    public Entity Entity { get; }
    public string Message { get; }
    public object? Value { get; }
}