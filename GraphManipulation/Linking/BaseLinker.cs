using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Linking;

public abstract class BaseLinker<TValue1, TValue2, TIdentifier1, TIdentifier2> 
    : ILinker<TValue1, TValue2, TIdentifier1, TIdentifier2>
    where TValue1 : IModel<TIdentifier1>
    where TValue2 : IModel<TIdentifier2>
{
    public abstract void AddLink(TIdentifier1 first, TIdentifier2 second);
    
    public void AddLink(TValue1 first, TValue2 second) => AddLink(first.Identifier, second.Identifier);
    
    public abstract void DeleteLink(TIdentifier1 first, TIdentifier2 second);

    public void DeleteLink(TValue1 first, TValue2 second) => DeleteLink(first.Identifier, second.Identifier);
    
    public abstract IEnumerable<TValue1> GetLinkedEntities(TIdentifier2 identifier);

    public IEnumerable<TValue1> GetLinkedEntities(TValue2 entity) => GetLinkedEntities(entity.Identifier);

    public abstract IEnumerable<TValue2> GetLinkedEntities(TIdentifier1 identifier);
    
    public IEnumerable<TValue2> GetLinkedEntities(TValue1 entity) => GetLinkedEntities(entity.Identifier);
}