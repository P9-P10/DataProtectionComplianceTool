using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public class OriginManager<T> : IOriginManager<T>
{
    public void Add(Origin<T> value)
    {
        throw new NotImplementedException();
    }

    public void Update(T id, Origin<T> value)
    {
        throw new NotImplementedException();
    }

    public void Delete(T id)
    {
        throw new NotImplementedException();
    }

    public Origin<T> Get(T id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Origin<T>> GetAll()
    {
        throw new NotImplementedException();
    }
}