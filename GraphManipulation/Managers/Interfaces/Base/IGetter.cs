using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Managers.Interfaces.Base;

public interface IGetter<TResult, TKey>
{
    public IEnumerable<TResult> GetAll();
    public TResult? Get(TKey key);
}