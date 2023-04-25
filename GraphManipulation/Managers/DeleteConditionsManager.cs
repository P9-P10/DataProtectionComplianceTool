using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class DeleteConditionsManager : NamedEntityManager<DeleteCondition>, IDeleteConditionsManager
{
    public DeleteConditionsManager(IMapper<DeleteCondition> mapper) : base(mapper)
    {
    }

    public IEnumerable<IDeleteCondition> GetAll() => base.GetAll();
    public IDeleteCondition? Get(string key) => base.Get(key);

    public void UpdateDescription(string key, string description)
    {
        var condition = GetByName(key);
        condition.Description = description;
        _mapper.Update(condition);
    }

    public void Add(string name, string description, string condition)
    {
        _mapper.Insert(new DeleteCondition() { Name = name, Description = description, Condition = condition });
    }

    public void UpdateCondition(string name, string condition)
    {
        var deleteCondition = GetByName(name);
        deleteCondition.Condition = condition;
        _mapper.Update(deleteCondition);
    }
}