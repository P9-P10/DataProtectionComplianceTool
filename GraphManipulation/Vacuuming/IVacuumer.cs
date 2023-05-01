using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "");

    public IEnumerable<DeletionExecution> Execute();

    public IEnumerable<DeletionExecution> ExecuteVacuumingRules(IEnumerable<VacuumingRule> vacuumingRules);
}