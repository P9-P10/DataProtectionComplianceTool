namespace GraphManipulation.Vacuuming;

public interface IVacuumerStore
{
    public int StoreVacuumingRule(VacuumingRule vacuumingRule);

    public IEnumerable<VacuumingRule> FetchVacuumingRules();

    public VacuumingRule FetchVacuumingRule(int id);

    public void DeleteVacuumingRule(int id);

    public bool UpdateVacuumingRule(int id, VacuumingRule newRule);
}