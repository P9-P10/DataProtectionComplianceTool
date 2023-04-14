using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class MetadataManager : IMetadataManager<int>
{
    public void Add(Metadata<int> value)
    {
        throw new NotImplementedException();
    }

    public void Update(int id, Metadata<int> value)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Metadata<int> Get(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Metadata<int>> GetAll()
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

    public IEnumerable<Metadata<int>> GetMetadataWithNullValues()
    {
        throw new NotImplementedException();
    }
}