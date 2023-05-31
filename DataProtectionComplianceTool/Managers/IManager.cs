namespace GraphManipulation.Managers;

public interface IManager<TKey, TValue>
{
    public bool Create(TKey key);
    public bool Update(TKey key, TValue value);
    public bool Delete(TKey key);
    public IEnumerable<TValue> GetAll();
    public TValue? Get(TKey key);
}