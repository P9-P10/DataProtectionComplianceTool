namespace GraphManipulation.Managers.Interfaces;

public interface ILinkManager<T1, T2>
{
    public void AddLink(T1 entityA, T2 entityB);
    public void DeleteLink(T1 entityA, T2 entityB);
    public IEnumerable<T1> GetLinkedEntities(T2 entity);
    public IEnumerable<T2> GetLinkedEntities(T1 entity);
}