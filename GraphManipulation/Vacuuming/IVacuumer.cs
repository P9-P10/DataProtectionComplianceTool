using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "");

    public IEnumerable<DeletionExecution> Execute();

    public IEnumerable<DeletionExecution> RunVacuumingRules(IEnumerable<VacuumingRule> rules);
    public void RunAllVacuumingRules();
    public VacuumingRule AddVacuumingRule(string ruleName, string purpose, string interval, List<Purpose>? purposes);

    public void UpdateVacuumingRule(VacuumingRule vacuumingRule, string newName = "", string newDescription = "",
        string newInterval = "");
    
    public VacuumingRule GetVacuumingRule(int ruleId);


    public IEnumerable<VacuumingRule> GetAllVacuumingRules();
}