using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Managers;

public class Manager<TKey, TValue> : IManager<TKey, TValue>
    where TKey : notnull
    where TValue : Entity<TKey>, new() 
{
    private readonly IMapper<TValue> _mapper;

    public Manager(IMapper<TValue> mapper)
    {
        _mapper = mapper;
    }

    public bool Create(TKey key)
    {
        var old = Get(key);
        if (old is not null) return false;
        
        _mapper.Insert(new TValue { Key = key });
        return true;
    }

    public bool Update(TKey key, TValue value)
    {
        var old = Get(key);
        if (old is null) return false;
        
        old.Fill(value);

        _mapper.Update(value);
        return true;
    }

    public bool Delete(TKey key)
    {
        var entity = Get(key);
        if (entity is null) return false;

        _mapper.Delete(entity);
        return true;
    }

    public TValue? Get(TKey key)
    {
        return _mapper.FindSingle(entity => entity.Key!.Equals(key));
    }

    public IEnumerable<TValue> GetAll()
    {
        return _mapper.Find(_ => true);
    }
}