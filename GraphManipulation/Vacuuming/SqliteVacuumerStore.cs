using Dapper;
using Microsoft.Data.Sqlite;

namespace GraphManipulation.Vacuuming;

public class SqliteVacuumerStore : IVacuumerStore
{
    private readonly SqliteConnection _dbConnection;

    public SqliteVacuumerStore(SqliteConnection dbConnection)
    {
        _dbConnection = dbConnection;
        Initiate();
    }

    private void Initiate()
    {
        _dbConnection.Execute(
            "CREATE TABLE IF NOT EXISTS vacuuming_rules " +
            "(id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "ruleName VARCHAR," +
            "purpose  VARCHAR, " +
            "interval VARCHAR);");
    }

    private int Store(VacuumingRule? vacuumingRule)
    {
        string query =
            $"INSERT INTO vacuuming_rules (ruleName,purpose,interval) " +
            $"VALUES ('{vacuumingRule.RuleName}'," +
            $"'{vacuumingRule.Purpose}'," +
            $"'{vacuumingRule.Interval}') RETURNING id;";
        IEnumerable<int> result = _dbConnection.Query<int>(query);

        return result.ToArray()[0];
    }

    public int StoreVacuumingRule(VacuumingRule? vacuumingRule)
    {
        return Store(vacuumingRule);
    }

    public IEnumerable<VacuumingRule> FetchVacuumingRules()
    {
        IEnumerable<VacuumingRule> result =
            _dbConnection.Query<VacuumingRule>(
                $"SELECT ruleName," +
                $"purpose," +
                $"interval " +
                $"FROM vacuuming_rules;");
        var vacuumingRules = result.ToList();
        return vacuumingRules;
    }


    /// <summary>
    /// Fetches vacuuming rule by ID if it exists.
    /// If no vacuuming rule with the id exists, it returns null.
    /// </summary>
    /// <param name="id">Id of the vacuuming rule</param>
    /// <returns>Vacuuming rule or null.</returns>
    public VacuumingRule? FetchVacuumingRule(int id)
    {
        IEnumerable<VacuumingRule> result =
            _dbConnection.Query<VacuumingRule>(
                $"SELECT ruleName," +
                $"purpose," +
                $"interval " +
                $"FROM vacuuming_rules WHERE id = {id};");
        var vacuumingRules = result.ToList();
        return vacuumingRules.Count() == 1 ? vacuumingRules.ToArray()[0] : null;
    }

    public void DeleteVacuumingRule(int id)
    {
        _dbConnection.Execute($"DELETE FROM vacuuming_rules WHERE id = {id};");
    }

    public bool UpdateVacuumingRule(int id, VacuumingRule? newRule)
    {
        int result = _dbConnection.Execute(
            $"UPDATE vacuuming_rules SET purpose = '{newRule.Purpose}',interval = '{newRule.Interval}',ruleName = '{newRule.RuleName}' WHERE id = {id};");
        return result > 0;
    }
}