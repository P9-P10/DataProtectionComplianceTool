using GraphManipulation.Logging;
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

    public IEnumerable<DeletionExecution> Execute()
    {
        throw new NotImplementedException();
    }

    IEnumerable<DeletionExecution> IVacuumer.GenerateUpdateStatement(string predefinedExpirationDate)
    {
        throw new NotImplementedException();
    }

    public void RunVacuumingRule(int ruleId)
    {
        throw new NotImplementedException();
    }

    public void RunAllVacuumingRules()
    {
        throw new NotImplementedException();
    }

    public void AddVacuumingRule(string rule)
    {
        throw new NotImplementedException();
    }

    public void UpdateVacuumingRule(int ruleId, string updatedRule)
    {
        throw new NotImplementedException();
    }

    public void DeleteVacuumingRule(int ruleId)
    {
        throw new NotImplementedException();
    }
}