using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Decorators;

public class LoggingVacuumer : LoggingDecorator<string, VacuumingRule>, IVacuumer
{
    private readonly ILogger _logger;
    private readonly IVacuumer _vacuumer;

    public LoggingVacuumer(IVacuumer vacuumer, ILogger logger) : base(logger)
    {
        _vacuumer = vacuumer;
        _logger = logger;
    }


    public IEnumerable<DeletionExecution> GenerateUpdateStatement()
    {
        return _vacuumer.GenerateUpdateStatement();
    }

    public IEnumerable<DeletionExecution> Execute()
    {
        return ExecuteAndLog(() => _vacuumer.Execute());
    }

    public IEnumerable<DeletionExecution> ExecuteVacuumingRuleList(IEnumerable<VacuumingRule> vacuumingRules)
    {
        return ExecuteAndLog(() => _vacuumer.ExecuteVacuumingRuleList(vacuumingRules));
    }

    public IEnumerable<DeletionExecution> ExecuteVacuumingRule(VacuumingRule vacuumingRule)
    {
        return ExecuteAndLog(() => _vacuumer.ExecuteVacuumingRule(vacuumingRule));
    }

    private IEnumerable<DeletionExecution> ExecuteAndLog(Func<IEnumerable<DeletionExecution>> executeFunc)
    {
        var executions = executeFunc().ToList();
        executions.ForEach(execution => LogExecute(execution.VacuumingRule.Key));

        CreateDeletionExecutionLogs(executions).ToList().ForEach(_logger.Append);
        return executions;
    }

    private static IEnumerable<IMutableLog> CreateDeletionExecutionLogs(IEnumerable<DeletionExecution> executions)
    {
        return executions.Select(execution =>
        {
            var subject = new TableColumnPair(execution.Table, execution.Column).ToString();
            var message = CreateDeleteExecutionLogMessage(execution);
            return new MutableLog(LogType.Vacuuming, subject, LogMessageFormat.Plaintext, message);
        });
    }

    private static string CreateDeleteExecutionLogMessage(DeletionExecution execution)
    {
        return $"\"{execution.Query}\" possibly affected ({execution.Table}, {execution.Column}) " +
               $"because it is stored under the following purpose(s): {string.Join(", ", execution.Purposes.Select(p => p.ToListingIdentifier()))}";
    }
}