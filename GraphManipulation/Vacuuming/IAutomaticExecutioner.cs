using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public interface IAutomaticExecutioner
{
    public void AddNewAutomaticExecution(string policy, string executeEvery,DateTime startDate);

    public void UpdateAutomaticExecution();

    public Execution GetNextExecution();

    public List<Execution> GetAutomaticExecutions();
}