using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Managers.Interfaces;

public interface IManager<TKey, TValue> : IGetter<TValue, TKey> 
{
    public bool Create(TKey key);
    public bool Update(TKey key, TValue value);
    public bool Delete(TKey key);
}