using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using IOriginsManager = GraphManipulation.Managers.Interfaces.IOriginsManager;

namespace GraphManipulation.Decorators.Managers;

public class OriginsManagerDecorator : LoggingDecorator, IOriginsManager
{
    private readonly IManager<string, Origin> _manager;
    public OriginsManagerDecorator(IManager<string, Origin> manager, ILogger logger) : base(logger, "origin")
    {
        _manager = manager;
    }

    public bool Create(string key)
    {
        LogCreate(key, new object());
        return _manager.Create(key);
    }

    public bool Update(string key, Origin value)
    {
        LogUpdate(key, new object());
        return _manager.Update(key, value);
    }

    public bool Delete(string key)
    {
        LogDelete(key);
        return _manager.Delete(key);
    }

    public IEnumerable<Origin> GetAll()
    {
        return _manager.GetAll();
    }

    public Origin? Get(string key)
    {
        return _manager.Get(key);
    }
}