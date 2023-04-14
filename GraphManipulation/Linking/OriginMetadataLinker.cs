using GraphManipulation.Models;

namespace GraphManipulation.Linking;

public class OriginMetadataLinker<T1, T2> : BaseLinker<Origin<T1>, Metadata<T2>, T1, T2>
{
    public override void AddLink(T1 first, T2 second)
    {
        throw new NotImplementedException();
    }

    public override void DeleteLink(T1 first, T2 second)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Origin<T1>> GetLinkedEntities(T2 identifier)
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<Metadata<T2>> GetLinkedEntities(T1 identifier)
    {
        throw new NotImplementedException();
    }
}