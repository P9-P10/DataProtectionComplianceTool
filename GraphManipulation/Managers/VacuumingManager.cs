using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Managers;

public class VacuumingManager<T> : IVacuumingManager<T>
{
    public void Add(VacuumingRule<T> value)
    {
        throw new NotImplementedException();
    }

    public void Update(T id, VacuumingRule<T> value)
    {
        throw new NotImplementedException();
    }

    public void Delete(T id)
    {
        throw new NotImplementedException();
    }

    public VacuumingRule<T> Get(T id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VacuumingRule<T>> GetAll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DeletionExecution> Execute(T id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DeletionExecution> ExecuteAll()
    {
        throw new NotImplementedException();
    }
}