using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Managers;

public class Manager<TKey, TValue> : IManager<TKey, TValue>
    where TKey : notnull
    where TValue : Entity<TKey>, new() 
{
    private readonly IMapper<TValue> _mapper;

    protected Manager(IMapper<TValue> mapper)
    {
        _mapper = mapper;
    }

    public void Create(TKey key)
    {
        _mapper.Insert(new TValue { Key = key });
    }

    public void Update(TKey key, TValue value)
    {
        var old = Get(key);
        if (old is null) return;

        value.Id = old.Id;

        _mapper.Update(value);
    }

    public void Delete(TKey key)
    {
        var entity = Get(key);
        if (entity is null) return;

        _mapper.Delete(entity);
    }

    public TValue? Get(TKey key)
    {
        return _mapper.FindSingle(entity => entity.Key.Equals(key));
    }

    public IEnumerable<TValue> GetAll()
    {
        return _mapper.Find(_ => true);
    }
}