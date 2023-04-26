using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Decorators.Managers;

public class DeleteConditionsManagerDecorator : LoggingDecorator, IDeleteConditionsManager
{

    private readonly IDeleteConditionsManager _manager;
    
    public DeleteConditionsManagerDecorator(IDeleteConditionsManager manager, ILogger logger) : base(logger, "deletecondition")
    {
        _manager = manager;
    }

    public IEnumerable<IDeleteCondition> GetAll()
    {
        return _manager.GetAll();
    }

    public IDeleteCondition? Get(string key)
    {
        return _manager.Get(key);
    }

    public void Delete(string key)
    {
        LogDelete(key);
        _manager.Delete(key);
    }

    public void UpdateName(string name, string newName)
    {
        LogUpdate(name, new {Name = newName});
        _manager.UpdateName(name, newName);
    }

    public void UpdateDescription(string key, string description)
    {
        LogUpdate(key, new {Description = description});
        _manager.UpdateDescription(key, description);
    }

    public void Add(string name, string description, string condition)
    {
        LogAdd(name, new {Description = description, Condition = condition});
        _manager.Add(name, description, condition);
    }

    public void UpdateCondition(string name, string condition)
    {
        LogUpdate(name, new {Condition = condition});
        _manager.UpdateCondition(name, condition);
    }
}