using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Managers;

public class VacuumingManager : IVacuumingManager<int>
{
    public void Add(VacuumingRule<int> value)
    {
        throw new NotImplementedException();
    }

    public void Update(int id, VacuumingRule<int> value)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public VacuumingRule<int> Get(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VacuumingRule<int>> GetAll()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DeletionExecution> Execute(int vacuumingRule)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DeletionExecution> ExecuteAll()
    {
        throw new NotImplementedException();
    }
}