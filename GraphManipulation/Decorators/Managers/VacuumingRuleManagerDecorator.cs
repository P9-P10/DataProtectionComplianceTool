using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Decorators.Managers;

public class VacuumingRuleManagerDecorator : LoggingDecorator, IVacuumingRulesManager
{
    private readonly IVacuumingRulesManager _manager;

    public VacuumingRuleManagerDecorator(IVacuumingRulesManager manager, ILogger logger) : base(logger,
        "vacuuming rule")
    {
        _manager = manager;
    }

    public IEnumerable<IVacuumingRule> GetAll()
    {
        return _manager.GetAll();
    }

    public IVacuumingRule? Get(string key)
    {
        return _manager.Get(key);
    }

    public void Delete(string key)
    {
        LogDelete(key);
        _manager.Delete(key);
    }

    public void UpdateDescription(string key, string description)
    {
        LogUpdate(key, new {Description = description});
        _manager.UpdateDescription(key, description);
    }

    public void UpdateName(string name, string newName)
    {
        LogUpdate(name, new {Name = newName});
        _manager.UpdateName(name, newName);
    }

    public void AddVacuumingRule(string name, string interval, string purposeName)
    {
        LogUpdate(name, new {Interval = interval, Purpose = purposeName});
        _manager.AddVacuumingRule(name, interval, purposeName);
    }

    public void UpdateInterval(string name, string interval)
    {
        LogUpdate(name, new {Interval = interval});
        _manager.UpdateInterval(name, interval);
    }

    public void ExecuteRule(string name)
    {
        _manager.ExecuteRule(name);
    }

    public void AddPurpose(string name, string purposeName)
    {
        LogUpdate(name, new {Purpose = purposeName});
        _manager.AddPurpose(name, purposeName);
    }

    public void RemovePurpose(string name, string purposeName)
    {
        LogUpdate(name, new {purposeName});
        _manager.RemovePurpose(name, purposeName);
    }
}