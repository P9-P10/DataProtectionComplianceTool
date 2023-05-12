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
        return ExecuteAndLog(() => _vacuumer.Execute());
    }

    public IEnumerable<DeletionExecution> ExecuteVacuumingRules(IEnumerable<VacuumingRule> vacuumingRules)
    {
        return ExecuteAndLog(() => _vacuumer.ExecuteVacuumingRules(vacuumingRules));
    }

    private IEnumerable<DeletionExecution> ExecuteAndLog(Func<IEnumerable<DeletionExecution>> executeFunc)
    {
        var executions = executeFunc().ToList();
        CreateDeletionExecutionLogs(executions).ToList().ForEach(_logger.Append);
        return executions;
    }

    private static IEnumerable<IMutableLog> CreateDeletionExecutionLogs(IEnumerable<DeletionExecution> executions)
    {
        return executions.Select(execution =>
        {
            var subject = new TableColumnPair(execution.Table, execution.Column).ToListingIdentifier();
            var message = CreateDeleteExecutionLogMessage(execution);
            return new MutableLog(LogType.Vacuuming, subject, LogMessageFormat.Plaintext, message);
        });
    }

    private static string CreateDeleteExecutionLogMessage(DeletionExecution execution)
    {
        return $"\"{execution.Query}\" affected ({execution.Table}, {execution.Column}) " +
               $"because it is stored under the following purpose(s): {string.Join(", ", execution.Purposes.Select(p => p.ToListingIdentifier()))}";
    }
}