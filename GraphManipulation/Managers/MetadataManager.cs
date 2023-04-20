using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class MetadataManager<T> : IMetadataManager<T>
{
    public void Add(Metadata<T> value)
    {
        throw new NotImplementedException();
    }

    public void Update(T id, Metadata<T> value)
    {
        throw new NotImplementedException();
    }

    public void Delete(T id)
    {
        throw new NotImplementedException();
    }

    public Metadata<T> Get(T id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Metadata<T>> GetAll()
    {
        throw new NotImplementedException();
    }

    public void CreateMetadataTables()
    {
        throw new NotImplementedException();
    }

    public void DropMetadataTables()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Metadata<T>> GetMetadataWithNullValues()
    {
        throw new NotImplementedException();
    }
}