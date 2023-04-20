using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Managers.Interfaces;

public interface IVacuumingManager<T> : IManager<VacuumingRule<T>, T>
{
    public IEnumerable<DeletionExecution> Execute(T id);
    public IEnumerable<DeletionExecution> ExecuteAll();
}