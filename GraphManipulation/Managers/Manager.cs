using GraphManipulation.DataAccess;
using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public class Manager<TKey, TValue> : IManager<TKey, TValue>
    where TKey : notnull
    where TValue : Entity<TKey>, new() 
{
    protected readonly IMapper<TValue> Mapper;

    public Manager(IMapper<TValue> mapper)
    {
        Mapper = mapper;
    }

    public bool Create(TKey key)
    {
        var old = Get(key);
        if (old is not null) return false;
        
        Mapper.Insert(new TValue { Key = key });
        return true;
    }

    public bool Update(TKey key, TValue value)
    {
        var old = Get(key);
        if (old is null) return false;
        
        old.UpdateUsing(value);

        Mapper.Update(old);
        return true;
    }

    public bool Delete(TKey key)
    {
        var entity = Get(key);
        if (entity is null) return false;

        Mapper.Delete(entity);
        return true;
    }

    public TValue? Get(TKey key)
    {
        return Mapper.FindSingle(entity => key.Equals(entity.Key!));
    }

    public IEnumerable<TValue> GetAll()
    {
        return Mapper.Find(_ => true);
    }
}