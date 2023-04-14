namespace GraphManipulation.Managers.Interfaces;

public interface ILinkManager<T1, T2>
{
    public void AddLink(T1 from, T2 to);
    public void DeleteLink(T1 from, T2 to);
    public IEnumerable<T1> GetLinkedFromEntities(T2 to);
    public IEnumerable<T2> GetLinkedToEntities(T1 from);
}