using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Services;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly IPersonDataColumnService _personDataColumnService;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IVacuumerStore _vacuumerStore;

    public Vacuumer(IPersonDataColumnService personDataColumnService, IQueryExecutor queryExecutor,
        IVacuumerStore vacuumerStore)
    {
        _personDataColumnService = personDataColumnService;
        _queryExecutor = queryExecutor;
        _vacuumerStore = vacuumerStore;
    }

    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        return _personDataColumnService.GetColumns().Select(CreateDeletionExecution).ToList();
    }

    private DeletionExecution CreateDeletionExecution(PersonDataColumn personDataColumn)
    {
        DeletionExecution deletionExecution = new();
        var logicOperator = " AND ";
        string query = CreateUpdateQuery(personDataColumn);

        query += AppendConditions(personDataColumn, logicOperator, deletionExecution);

        deletionExecution.Column = personDataColumn.ColumnName;
        deletionExecution.Table = personDataColumn.TableName;

        deletionExecution.Query = ReplaceLastOccurrenceOfString(query, logicOperator);
        return deletionExecution;
    }

    private static string AppendConditions(PersonDataColumn personDataColumn, string logicOperator,
        DeletionExecution deletionExecution)
    {
        string conditionalStatement = "";
        foreach (DeleteCondition deleteCondition in personDataColumn.DeleteConditions)
        {
            conditionalStatement += $"({deleteCondition.Condition})";
            conditionalStatement += logicOperator;
            deletionExecution.AddPurpose(deleteCondition.Purpose);
        }

        return conditionalStatement;
    }

    private static string CreateUpdateQuery(PersonDataColumn personDataColumn)
    {
        return
            $"UPDATE {personDataColumn.TableName} SET {personDataColumn.ColumnName} = {personDataColumn.DefaultValue} WHERE ";
    }

    public IEnumerable<DeletionExecution> Execute()
    {
        IEnumerable<DeletionExecution> executions = GenerateUpdateStatement();
        var deletionExecutions = executions.ToList();
        foreach (var deletionExecution in deletionExecutions)
        {
            _queryExecutor.Execute(deletionExecution.Query);
        }

        return deletionExecutions;
    }

    /// <summary>
    /// This function executes a specified vacuuming rule.
    /// It does not vacuum data if its protected by other purposes.
    /// </summary>
    /// <param name="ruleId">ID of the vacuuming rule</param>
    /// <returns></returns>
    public IEnumerable<DeletionExecution> RunVacuumingRule(int ruleId)
    {
        List<DeletionExecution> conditions = new List<DeletionExecution>();
        VacuumingRule? rule = _vacuumerStore.FetchVacuumingRule(ruleId);

        List<PersonDataColumn> personDataColumns = _personDataColumnService.GetColumns().ToList();

        foreach (var personDataColumn in personDataColumns)
        {
            bool containsCorrectCondition = ContainsCorrectCondition(personDataColumn, rule);
            if (!containsCorrectCondition) continue;

            DeletionExecution execution = CreateDeletionExecution(personDataColumn);
            conditions.Add(execution);
            _queryExecutor.Execute(execution.Query);
        }

        return conditions;
    }

    private static bool ContainsCorrectCondition(PersonDataColumn personDataColumn, VacuumingRule? rule)
    {
        return personDataColumn.DeleteConditions.Any(x => x.Purpose == rule.Purpose);
    }

    /// <summary>
    /// Same as Execute
    /// </summary>
    public void RunAllVacuumingRules()
    {
        // TODO Find ud af om vi kalder den RunAll eller Execute.
        Execute();
    }

    public int AddVacuumingRule(string ruleName, string purpose, string interval)
    {
        VacuumingRule? vacuumingRule = new(ruleName, purpose, interval);
        return _vacuumerStore.StoreVacuumingRule(vacuumingRule);
    }

    public void UpdateVacuumingRule(int ruleId, string newRuleName = "", string newPurpose = "",
        string newInterval = "")
    {
        VacuumingRule? rule = _vacuumerStore.FetchVacuumingRule(ruleId);
        if (newRuleName != "")
        {
            rule.RuleName = newRuleName;
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

    public string GetVacuumingRule(int ruleId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetAllVacuumingRules()
    {
        throw new NotImplementedException();
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