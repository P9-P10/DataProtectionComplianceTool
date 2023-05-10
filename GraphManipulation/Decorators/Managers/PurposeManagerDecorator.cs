using GraphManipulation.Logging;
using GraphManipulation.Logging.Operations;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Decorators.Managers;

public class PurposeManagerDecorator : LoggingDecorator, IPurposesManager
{
    private readonly IPurposesManager _manager;

    public PurposeManagerDecorator(IPurposesManager manager, ILogger logger) : base(logger, "purpose")
    {
        _manager = manager;
    }

    public IEnumerable<IPurpose> GetAll() => _manager.GetAll();
    public IPurpose? Get(string key) => _manager.Get(key);

    public void Delete(string key)
    {
        LogDelete(key);
        _manager.Delete(key);
    }

    public void UpdateName(string name, string newName)
    {
        LogUpdate(name, new { Name = newName });
        _manager.UpdateName(name, newName);
    }

    public void UpdateDescription(string key, string description)
    {
        LogUpdate(key, new {Description = description});
        _manager.UpdateDescription(key, description);
    }

    public void Add(string name, bool legallyRequired, string description)
    {
        LogAdd(name, new {LegallyRequired = legallyRequired, Description = description});
        _manager.Add(name, legallyRequired, description);
    }

    public void UpdateLegallyRequired(string name, bool legallyRequired)
    {
        LogUpdate(name, new {LegallyRequired = legallyRequired});
        _manager.UpdateLegallyRequired(name, legallyRequired);
    }

    public void SetDeleteCondition(string purposeName, string deleteConditionName)
    {
        LogUpdate(purposeName, new {DeleteCondition = deleteConditionName});
        _manager.SetDeleteCondition(purposeName, deleteConditionName);
    }
}