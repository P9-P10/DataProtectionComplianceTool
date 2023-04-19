using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;

namespace GraphManipulation.Managers.Interfaces;

public interface IVacuumingRuleManager : 
    IGetter<VacuumingRule, string>, 
    IDeleter<string>, 
    INameUpdater
{
    public void AddVacuumingRule(string name, string interval, string purposeName);
    public void UpdateInterval(string name, string interval);
}