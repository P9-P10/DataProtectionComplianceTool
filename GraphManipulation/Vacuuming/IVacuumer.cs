namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{

    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "");

    public IEnumerable<DeletionExecution> Execute();

    
    public void RunVacuumingRule(int ruleId);
    public void RunAllVacuumingRules();
    public void AddVacuumingRule(string rule);
    public void UpdateVacuumingRule(int ruleId, string updatedRule);

}