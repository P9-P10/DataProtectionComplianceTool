namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{

    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "");

    public IEnumerable<DeletionExecution> Execute();

    
    public IEnumerable<DeletionExecution> RunVacuumingRule(int ruleId);
    public void RunAllVacuumingRules();
    public int AddVacuumingRule(string ruleName, string purpose, string interval);
    public void UpdateVacuumingRule(int ruleId, string newRuleName, string newPurpose, string newInterval);

}