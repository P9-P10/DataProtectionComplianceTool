namespace GraphManipulation.Managers.Interfaces;

public interface ILinkManager<T1, T2>
{
    public void AddLink(T1 key1, T2 key2);
    public void DeleteLink(T1 key1, T2 key2);
    public IEnumerable<T1> GetLinkedEntities(T2 key);
    public IEnumerable<T2> GetLinkedEntities(T1 key);
}