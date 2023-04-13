namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public List<string> GenerateUpdateStatement(string predefinedExpirationDate = "");
    
    public void RunVacuumingRule(int ruleId);
    public void RunAllVacuumingRules();
    public void AddVacuumingRule(string rule);
    public void UpdateVacuumingRule(int ruleId, string updatedRule);
    public void DeleteVacuumingRule(int ruleId);

}