using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public interface IManager<TValue, TIdentifier>
{
    public void Add(TValue value);
    public void Update(TIdentifier id, TValue value);
    public void Delete(TIdentifier id);
    public TValue Get(TIdentifier id);
    public IEnumerable<TValue> GetAll();
}