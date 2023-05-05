using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Decorators;

public class LoggingVacuumer : IVacuumer
{
    private readonly IVacuumer _vacuumer;
    private readonly ILogger _logger;

    public LoggingVacuumer(IVacuumer vacuumer, ILogger logger)
    {
        _vacuumer = vacuumer;
        _logger = logger;
    }


    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        return _vacuumer.GenerateUpdateStatement(predefinedExpirationDate);
    }

    public IEnumerable<DeletionExecution> Execute()
    {
        var executions = _vacuumer.Execute().ToList();

        CreateDeletionExecutionLogs(executions).ToList().ForEach(_logger.Append);
        
        return executions;
    }

    public IEnumerable<DeletionExecution> ExecuteVacuumingRules(IEnumerable<VacuumingRule> vacuumingRules)
    {
        var executions = _vacuumer.ExecuteVacuumingRules(vacuumingRules).ToList();

        CreateDeletionExecutionLogs(executions).ToList().ForEach(_logger.Append);
        
        return executions;
    }

    private static IEnumerable<IMutableLog> CreateDeletionExecutionLogs(IEnumerable<DeletionExecution> executions)
    {
        return executions.Select(execution =>
        {
            var subject = new TableColumnPair(execution.Table, execution.Column).ToListingIdentifier();
            var message = CreateDeleteExecutionLogMessage(execution, subject);
            return new MutableLog(LogType.Vacuuming, subject, LogMessageFormat.Plaintext, message);
        });
    }

    private static string CreateDeleteExecutionLogMessage(DeletionExecution execution, string subject)
    {
        return $"{execution.Query} affected {subject} " +
               $"because it is stored under: {string.Join(", ", execution.Purposes.Select(p => p.GetName()))}";
    }
}