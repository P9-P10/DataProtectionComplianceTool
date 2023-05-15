namespace GraphManipulation.Managers.Interfaces;

public interface IGetter<TValue, TKey>
{
    public IEnumerable<TValue> GetAll();
    public TValue? Get(TKey key);
}