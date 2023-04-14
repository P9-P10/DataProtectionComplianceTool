using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Linking;

public interface ILinker<TValue1, TValue2, in TIdentifier1, in TIdentifier2> 
    where TValue1 : IModel<TIdentifier1>
    where TValue2 : IModel<TIdentifier2>
{
    public void AddLink(TIdentifier1 first, TIdentifier2 second);
    public void AddLink(TValue1 first, TValue2 second);
    public void DeleteLink(TIdentifier1 first, TIdentifier2 second);
    public void DeleteLink(TValue1 first, TValue2 second);

    public IEnumerable<TValue1> GetLinkedEntities(TIdentifier2 identifier);
    public IEnumerable<TValue1> GetLinkedEntities(TValue2 entity);
    public IEnumerable<TValue2> GetLinkedEntities(TIdentifier1 identifier);
    public IEnumerable<TValue2> GetLinkedEntities(TValue1 entity);
}