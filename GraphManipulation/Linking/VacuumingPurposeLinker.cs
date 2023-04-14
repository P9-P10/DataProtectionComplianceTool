using GraphManipulation.Models;

namespace GraphManipulation.Linking;

public class VacuumingPurposeLinker<T1, T2> : BaseLinker<VacuumingRule<T1>, Purpose<T2>, T1, T2>
{
    public override void AddLink(T1 first, T2 second)
    {
        throw new NotImplementedException();
    }

    public override void DeleteLink(T1 first, T2 second)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<VacuumingRule<T1>> GetLinkedEntities(T2 identifier)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Purpose<T2>> GetLinkedEntities(T1 identifier)
    {
        throw new NotImplementedException();
    }
}