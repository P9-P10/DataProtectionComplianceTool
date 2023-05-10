namespace GraphManipulation.Managers.Interfaces.Base;

public interface IGetter<TValue, TKey>
{
    public IEnumerable<TValue> GetAll();
    public TValue? Get(TKey key);
}