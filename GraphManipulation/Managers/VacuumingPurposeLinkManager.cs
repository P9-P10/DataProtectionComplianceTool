using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public class VacuumingPurposeLinkManager<T1, T2> : ILinkManager<VacuumingRule<T1>, Purpose<T2>>
{
    public void AddLink(VacuumingRule<T1> entityA, Purpose<T2> entityB)
    {
        throw new NotImplementedException();
    }

    public void DeleteLink(VacuumingRule<T1> entityA, Purpose<T2> entityB)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VacuumingRule<T1>> GetLinkedEntities(Purpose<T2> entity)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Purpose<T2>> GetLinkedEntities(VacuumingRule<T1> entity)
    {
        throw new NotImplementedException();
    }
}