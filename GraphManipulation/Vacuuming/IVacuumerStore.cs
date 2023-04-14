namespace GraphManipulation.Vacuuming;

public interface IVacuumerStore
{
    /// <summary>
    /// Stores a new vacuuming rule in the system.
    /// </summary>
    /// <param name="vacuumingRule"></param>
    /// <returns>Returns the ID of the newly inserted rule.</returns>
    public int StoreVacuumingRule(VacuumingRule? vacuumingRule);

    
    /// <summary>
    /// Fetches existing Vacuuming rules.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VacuumingRule> FetchVacuumingRules();

    /// <summary>
    /// Fetches vacuuming rule by ID if it exists.
    /// If no vacuuming rule with the id exists, it returns null.
    /// </summary>
    /// <param name="id">Id of the vacuuming rule</param>
    /// <returns>Vacuuming rule or null.</returns>
    public VacuumingRule? FetchVacuumingRule(int id);

    /// <summary>
    /// Deletes vacuuming rule from the system.
    /// </summary>
    /// <param name="id">Id of the vacuuming rule to delete</param>
    public void DeleteVacuumingRule(int id);

    public bool UpdateVacuumingRule(int id, VacuumingRule? newRule);
}