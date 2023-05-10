using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;
using GraphManipulation.Models.Interfaces;
using IOriginsManager = GraphManipulation.Managers.Interfaces.Archive.IOriginsManager;

namespace GraphManipulation.Decorators.Managers;

public class OriginsManagerDecorator : LoggingDecorator, IOriginsManager
{
    private readonly IOriginsManager _manager;
    public OriginsManagerDecorator(IOriginsManager manager, ILogger logger) : base(logger, "origin")
    {
        _manager = manager;
    }

    public IEnumerable<IOrigin> GetAll()
    {
        return _manager.GetAll();
    }

    public IOrigin? Get(string key)
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

    public void Add(string name, string description)
    {
        LogAdd(name, new {Description = description});
        _manager.Add(name, description);
    }
}