using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IVacuumingRulesManager :
    IGetter<IVacuumingRule, string>,
    IDeleter<string>,
    IDescriptionUpdater<string>,
    INameUpdater,
    IHasPurposes<string, string>
{
    public void AddVacuumingRule(string name, string interval, string purposeName);
    public void UpdateInterval(string name, string interval);
    public void ExecuteRule(string name);
}