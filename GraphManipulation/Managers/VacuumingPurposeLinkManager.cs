using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public class VacuumingPurposeLinkManager<T1, T2> : ILinkManager<VacuumingRule<T1>, Purpose<T2>, T1, T2>
{
    public void AddLink(VacuumingRule<T1> entity1, Purpose<T2> entity2)
    {
        throw new NotImplementedException();
    }

    public void AddLink(T1 entity1, Purpose<T2> entity2)
    {
        throw new NotImplementedException();
    }

    public void AddLink(VacuumingRule<T1> entity1, T2 entity2)
    {
        throw new NotImplementedException();
    }

    public void AddLink(T1 entity1, T2 entity2)
    {
        throw new NotImplementedException();
    }

    public void DeleteLink(VacuumingRule<T1> entity1, Purpose<T2> entity2)
    {
        throw new NotImplementedException();
    }

    public void DeleteLink(T1 entity1, Purpose<T2> entity2)
    {
        throw new NotImplementedException();
    }

    public void DeleteLink(VacuumingRule<T1> entity1, T2 entity2)
    {
        throw new NotImplementedException();
    }

    public void DeleteLink(T1 entity1, T2 entity2)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VacuumingRule<T1>> GetLinkedEntities(T2 identifier)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VacuumingRule<T1>> GetLinkedEntities(Purpose<T2> entity)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Purpose<T2>> GetLinkedEntities(T1 identifier)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Purpose<T2>> GetLinkedEntities(VacuumingRule<T1> entity)
    {
        throw new NotImplementedException();
    }
}