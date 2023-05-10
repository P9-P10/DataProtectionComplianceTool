using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Managers.Interfaces;

public interface IManager<TKey, TValue> : IGetter<TValue, TKey> 
    where TKey : notnull
    where TValue : Entity<TKey>, new()
{
    public void Create(TKey key);
    public void Update(TKey key, TValue value);
    public void Delete(TKey key);
}