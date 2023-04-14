using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface ILinkManager<TValue1, TValue2, TIdentifier1, TIdentifier2> 
    where TValue1 : IModel<TIdentifier1>
    where TValue2 : IModel<TIdentifier2>
{
    public void AddLink(TValue1 entity1, TValue2 entity2);
    public void AddLink(TIdentifier1 entity1, TValue2 entity2);
    public void AddLink(TValue1 entity1, TIdentifier2 entity2);
    public void AddLink(TIdentifier1 entity1, TIdentifier2 entity2);
    public void DeleteLink(TValue1 entity1, TValue2 entity2);
    public void DeleteLink(TIdentifier1 entity1, TValue2 entity2);
    public void DeleteLink(TValue1 entity1, TIdentifier2 entity2);
    public void DeleteLink(TIdentifier1 entity1, TIdentifier2 entity2);
    public IEnumerable<TValue1> GetLinkedEntities(TIdentifier2 identifier);
    public IEnumerable<TValue1> GetLinkedEntities(TValue2 entity);
    public IEnumerable<TValue2> GetLinkedEntities(TIdentifier1 identifier);
    public IEnumerable<TValue2> GetLinkedEntities(TValue1 entity);
}