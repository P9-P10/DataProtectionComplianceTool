using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public interface IVacuumingRuleManager
{
    public VacuumingRule AddVacuumingRule(string name, string interval, string purposeName);
    public VacuumingRule UpdateVacuumingRuleName(string name, string newName);
    public VacuumingRule UpdateVacuumingRuleInterval(string name, string interval);
    public void DeleteVacuumingRule(string name);
    public IEnumerable<VacuumingRule> GetAllVacuumingRules();
    public VacuumingRule? GetVacuumingRule(string name);

}