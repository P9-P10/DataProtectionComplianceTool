using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public IEnumerable<DeletionExecution> GenerateUpdateStatement();

    public IEnumerable<DeletionExecution> Execute();

    public IEnumerable<DeletionExecution> ExecuteVacuumingRuleList(IEnumerable<VacuumingRule> vacuumingRules);

    public IEnumerable<DeletionExecution> ExecuteVacuumingRule(VacuumingRule vacuumingRule);
}