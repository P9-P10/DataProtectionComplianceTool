using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Decorators.Managers;

public class ProcessingsManagerDecorator : LoggingDecorator, IProcessingsManager
{
    private readonly IProcessingsManager _manager;
    public ProcessingsManagerDecorator(IProcessingsManager manager, ILogger logger) : base(logger, "processing")
    {
        _manager = manager;
    }

    public IEnumerable<IProcessing> GetAll()
    {
        return _manager.GetAll();
    }

    public IProcessing? Get(string key)
    {
        return _manager.Get(key);
    }

    public void UpdateDescription(string key, string description)
    {
        LogUpdate(key, new {Description = description});
        _manager.UpdateDescription(key, description);
    }

    public void Delete(string key)
    {
        LogDelete(key);
        _manager.Delete(key);
    }

    public void UpdateName(string name, string newName)
    {
        LogUpdate(name, new {Name = name});
        _manager.UpdateName(name, newName);
    }

    public void AddProcessing(string name, TableColumnPair tableColumnPair, string purposeName, string description)
    {
        LogUpdate(name, new {Column = tableColumnPair.ToListing(), Purpose = purposeName, Description = description});
        _manager.AddProcessing(name, tableColumnPair, purposeName, description);
    }
}