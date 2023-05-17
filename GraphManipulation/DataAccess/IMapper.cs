namespace GraphManipulation.DataAccess;

public interface IMapper<T>
{
    public T Insert(T value);
    public IEnumerable<T> Find(Func<T, bool> condition);
    public T? FindSingle(Func<T, bool> condition);
    public T Update(T value);
    public void Delete(T value);

}