using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IMetadataManager<T> : IManager<Metadata<T>, T>
{
    public void CreateMetadataTables();

    public void DropMetadataTables();
    
    public IEnumerable<Metadata<T>> GetMetadataWithNullValues();
}