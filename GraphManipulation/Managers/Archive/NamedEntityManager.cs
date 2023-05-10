using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Managers.Archive;

public class NamedEntityManager<T> : IGetter<T, string>, IDeleter<string>, INameUpdater where T : INamedEntity, IListable
{
    protected IMapper<T> _mapper;

    public NamedEntityManager(IMapper<T> mapper)
    {
        _mapper = mapper;
    }
    public IEnumerable<T> GetAll()
    {
        return _mapper.Find(_ => true);
    }

    public T? Get(string key)
    {
        return GetByName(key);
    }

    public void Delete(string key)
    {
        _mapper.Delete(GetByName(key));
    }

    public void UpdateName(string name, string newName)
    {
        var entity = GetByName(name);
        entity.SetName(newName);
        _mapper.Update(entity);
    }
    
    protected T? GetByName(string name)
    {
        return _mapper.FindSingle(namedEntity => namedEntity.GetName() == name);
    }
}