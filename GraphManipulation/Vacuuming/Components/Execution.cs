namespace GraphManipulation.Vacuuming.Components;

public class Execution
{
    public string Policy;

    public string ExecutionInterval;

    public DateTime NextExecution;


    public Execution(string policy, string executionInterval, DateTime nextExecution)
    {
        Policy = policy;
        ExecutionInterval = executionInterval;
        NextExecution = nextExecution;
    }

    private sealed class ExecutionEqualityComparer : IEqualityComparer<Execution>
    {
        public bool Equals(Execution? x, Execution? y)
        {
            if (x.GetType() != y.GetType()) return false;
            return x.Policy == y.Policy && x.ExecutionInterval == y.ExecutionInterval &&
                   x.NextExecution.Equals(y.NextExecution);
        }

        public int GetHashCode(Execution obj)
        {
            return HashCode.Combine(obj.Policy, obj.ExecutionInterval, obj.NextExecution);
        }
    }

    public static IEqualityComparer<Execution> ExecutionComparer { get; } = new ExecutionEqualityComparer();
}