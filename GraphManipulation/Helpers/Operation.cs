namespace GraphManipulation.Helpers;

public class SystemAction
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
