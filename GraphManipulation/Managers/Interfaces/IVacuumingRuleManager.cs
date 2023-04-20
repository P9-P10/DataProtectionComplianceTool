using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IVacuumingRuleManager : 
    IGetter<IVacuumingRule, string>, 
    IDeleter<string>, 
    INameUpdater
{
    public void AddVacuumingRule(string name, string interval, string purposeName);
    public void UpdateInterval(string name, string interval);
}