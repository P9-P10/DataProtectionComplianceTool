using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public interface IExecutionParser
{
    public Execution ParseExecutionPolicy(string executionInterval,string policy);

    public string ParseExecution(Execution execution);
}