using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Managers.Interfaces.Archive;

public interface INamedEntityManager<T> : IGetter<T, string>, IDeleter<string>, INameUpdater 
    where T : INamedEntity, IListable
{
    
}