namespace GraphManipulation.Managers.Interfaces.Base;

public interface IDeleter<T>
{
    public void Delete(T key);
}