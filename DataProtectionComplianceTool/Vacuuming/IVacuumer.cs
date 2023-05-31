using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public IEnumerable<DeletionExecution> GenerateUpdateStatement();

    public IEnumerable<DeletionExecution> Execute();

    public IEnumerable<DeletionExecution> ExecuteVacuumingPolicyList(IEnumerable<VacuumingPolicy> vacuumingPolicies);

    public IEnumerable<DeletionExecution> ExecuteVacuumingPolicy(VacuumingPolicy vacuumingPolicy);
}