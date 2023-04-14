using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public class VacuumingPurposeLinkManager<T1, T2> : ILinkManager<VacuumingRule<T1>, Purpose<T2>>
{
    public void AddLink(VacuumingRule<T1> from, Purpose<T2> to)
    {
        throw new NotImplementedException();
    }

    public void DeleteLink(VacuumingRule<T1> from, Purpose<T2> to)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VacuumingRule<T1>> GetLinkedFromEntities(Purpose<T2> to)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Purpose<T2>> GetLinkedToEntities(VacuumingRule<T1> from)
    {
        throw new NotImplementedException();
    }
}