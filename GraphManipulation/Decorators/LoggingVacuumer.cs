using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Decorators;

public class LoggingVacuumer : IVacuumer
{
    private IVacuumer _vacuumer;
    private ILogger _logger;

    public LoggingVacuumer(IVacuumer vacuumer, ILogger logger)
    {
        _vacuumer = vacuumer;
        _logger = logger;
    }


    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DeletionExecution> Execute()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<DeletionExecution> ExecuteVacuumingRules(IEnumerable<VacuumingRule> vacuumingRules)
    {
        throw new NotImplementedException();
    }

    public void RunAllVacuumingRules()
    {
        throw new NotImplementedException();
    }

    public VacuumingRule AddVacuumingRule(string ruleName, string purpose, string interval, List<Purpose>? purposes)
    {
        throw new NotImplementedException();
    }

    public void UpdateVacuumingRule(VacuumingRule vacuumingRule, string newName = "", string newDescription = "",
        string newInterval = "")
    {
        throw new NotImplementedException();
    }

    public VacuumingRule GetVacuumingRule(int ruleId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VacuumingRule> GetAllVacuumingRules()
    {
        throw new NotImplementedException();
    }
}