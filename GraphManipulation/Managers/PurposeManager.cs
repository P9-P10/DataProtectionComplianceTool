using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class PurposeManager<T> : IPurposeManager<T>
{
    public void Add(Purpose<T> value)
    {
        throw new NotImplementedException();
    }

    public void Update(T id, Purpose<T> value)
    {
        throw new NotImplementedException();
    }

    public void Delete(T id)
    {
        throw new NotImplementedException();
    }

    public Purpose<T> Get(T id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Purpose<T>> GetAll()
    {
        throw new NotImplementedException();
    }
}