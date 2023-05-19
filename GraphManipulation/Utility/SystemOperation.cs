namespace GraphManipulation.Utility;

public static class SystemOperation
{
    public enum Operation
    {
        Updated,
        Deleted,
        Created,
        Executed
    }
    
    public static string OperationToString(Operation operation)
    {
        return operation.ToString().ToLower();
    }
}
