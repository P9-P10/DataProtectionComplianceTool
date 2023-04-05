using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public interface IExecutionsFetcher
{
    public List<Execution> FetchExecutions();

    public void StoreExecutions(List<Execution> executions);

    public void StoreExecution(Execution execution);
}