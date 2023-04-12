using GraphManipulation.Services;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly IPersonDataColumnService? _personDataColumnService;
    private readonly IQueryExecutor? _queryExecutor;
    private readonly IVacuumerStore _vacuumerStore;

    public Vacuumer(IPersonDataColumnService? personDataColumnService, IQueryExecutor? queryExecutor,
        IVacuumerStore vacuumerStore)
    {
        _personDataColumnService = personDataColumnService;
        _queryExecutor = queryExecutor;
        _vacuumerStore = vacuumerStore;
    }

    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        var executions = new List<DeletionExecution>();
        foreach (var personDataColumn in _personDataColumnService.GetColumns())
        {
            var currentExecution = new DeletionExecution();
            var query =
                $"UPDATE {personDataColumn.TableName} SET {personDataColumn.ColumnName} = {personDataColumn.DefaultValue} WHERE ";
            var logicOperator = " AND ";
            foreach (var deleteCondition in personDataColumn.DeleteConditions)
            {
                query +=
                    $"({deleteCondition.Condition})";
                query += logicOperator;
                currentExecution.AddPurpose(deleteCondition.Purpose);
            }

            currentExecution.Column = personDataColumn.ColumnName;
            currentExecution.Table = personDataColumn.TableName;

            currentExecution.Query = ReplaceLastOccurrenceOfString(query, logicOperator);


            executions.Add(currentExecution);
        }

        return executions;
    }

    public IEnumerable<DeletionExecution> Execute()
    {
        IEnumerable<DeletionExecution> executions = GenerateUpdateStatement();
        foreach (var deletionExecution in executions.ToList())
        {
            _queryExecutor.Execute(deletionExecution.Query);
        }

        return executions;
    }

    public void RunVacuumingRule(int ruleId)
    {
        throw new NotImplementedException();
    }

    public void RunAllVacuumingRules()
    {
        Execute();
    }

    public int AddVacuumingRule(string ruleName, string purpose, string interval)
    {
        VacuumingRule vacuumingRule = new(ruleName, purpose, interval);
        return _vacuumerStore.StoreVacuumingRule(vacuumingRule);
    }

    public void UpdateVacuumingRule(int ruleId, string newRuleName = "", string newPurpose = "",
        string newInterval = "")
    {
        VacuumingRule rule = _vacuumerStore.FetchVacuumingRule(ruleId);
        if (newRuleName != "")
        {
            rule.Rule = newRuleName;
        }

        if (newPurpose != "")
        {
            rule.Purpose = newPurpose;
        }

        if (newInterval != "")
        {
            rule.Interval = newInterval;
        }

        _vacuumerStore.UpdateVacuumingRule(ruleId, rule);
    }

    public void DeleteVacuumingRule(int ruleId)
    {
        _vacuumerStore.DeleteVacuumingRule(ruleId);
    }

    private string ReplaceLastOccurrenceOfString(string inputString, string occurrenceToReplace,
        string replaceWith = ";")
    {
        var place = inputString.LastIndexOf(occurrenceToReplace, StringComparison.Ordinal);

        return place == -1
            ? inputString
            : inputString.Remove(place, occurrenceToReplace.Length).Insert(place, replaceWith);
    }
}