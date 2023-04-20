using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Managers.Interfaces.Base;

public interface IGetter<TResult, TKey> where TResult : IListable
{
    public IEnumerable<TResult> GetAll();
    public TResult? Get(TKey key);
}