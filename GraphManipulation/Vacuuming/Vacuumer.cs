using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly IMapper<PersonalDataColumn> _personDataColumnMapper;
    private readonly IQueryExecutor _queryExecutor;
    private readonly IMapper<VacuumingRule> _vacuumingRuleMapper;

    public Vacuumer(IMapper<PersonalDataColumn> personDataColumnMapper, IQueryExecutor queryExecutor,
        IMapper<VacuumingRule> vacuumingRuleMapper)
    {
        _personDataColumnMapper = personDataColumnMapper;
        _queryExecutor = queryExecutor;
        _vacuumingRuleMapper = vacuumingRuleMapper;
    }

    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        return _personDataColumnMapper.Find(_ => true).Select(CreateDeletionExecution).ToList();
    }

    private DeletionExecution CreateDeletionExecution(PersonalDataColumn personDataColumn)
    {
        DeletionExecution deletionExecution = new();
        var logicOperator = " AND ";
        string query = CreateUpdateQuery(personDataColumn);

        query += AppendConditions(personDataColumn, logicOperator, deletionExecution);

        deletionExecution.Column = personDataColumn.TableColumnPair.ColumnName;
        deletionExecution.Table = personDataColumn.TableColumnPair.TableName;

        deletionExecution.Query = ReplaceLastOccurrenceOfString(query, logicOperator);
        return deletionExecution;
    }

    private static string AppendConditions(PersonalDataColumn personDataColumn, string logicOperator,
        DeletionExecution deletionExecution)
    {
        string conditionalStatement = "";
        foreach (Purpose purpose in personDataColumn.Purposes)
        {
            conditionalStatement += $"({purpose.DeleteCondition.Condition})";
            conditionalStatement += logicOperator;
            deletionExecution.AddPurpose(purpose);
        }

        return conditionalStatement;
    }

    private static string CreateUpdateQuery(PersonalDataColumn personDataColumn)
    {
        return
            $"UPDATE {personDataColumn.TableColumnPair.TableName} SET {personDataColumn.TableColumnPair.ColumnName} = {personDataColumn.DefaultValue} WHERE ";
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
    /// <param name="vacuumingRules"></param>
    /// <returns></returns>
    public IEnumerable<DeletionExecution> RunVacuumingRules(IEnumerable<VacuumingRule> vacuumingRules)
    {
        List<DeletionExecution> executions = new List<DeletionExecution>();

        List<PersonalDataColumn> personDataColumns = _personDataColumnMapper.Find(x=>true).ToList();

        foreach (var personDataColumn in personDataColumns)
        {
            foreach (var rule in vacuumingRules.ToList())
            {
                bool containsCorrectCondition = ContainsCorrectCondition(personDataColumn, rule);
                if (!containsCorrectCondition) continue;

                DeletionExecution execution = CreateDeletionExecution(personDataColumn);
                executions.Add(execution);
                _queryExecutor.Execute(execution.Query);
            }
           
        }

        return executions;
    }

    private static bool ContainsCorrectCondition(PersonalDataColumn personDataColumn, VacuumingRule? rule)
    {
        foreach (var purpose in personDataColumn.Purposes)
        {
            if (rule != null && rule.Purposes.Contains(purpose))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Same as Execute
    /// </summary>
    public void RunAllVacuumingRules()
    {
        // TODO Find ud af om vi kalder den RunAll eller Execute.
        Execute();
    }

    public VacuumingRule AddVacuumingRule(string ruleName, string interval, string description,
        List<Purpose>? purposes = null)
    {
        purposes ??= new List<Purpose>();
        VacuumingRule? vacuumingRule = new(description, ruleName, interval, purposes);
        return _vacuumingRuleMapper.Insert(vacuumingRule);
    }

    public void UpdateVacuumingRule(VacuumingRule vacuumingRule, string newName = "", string newDescription = "",
        string newInterval = "")
    {
        if (newName != "")
        {
            vacuumingRule.Name = newName;
        }

        if (newDescription != "")
        {
            vacuumingRule.Description = newDescription;
        }

        if (newInterval != "")
        {
            vacuumingRule.Interval = newInterval;
        }

        _vacuumingRuleMapper.Update(vacuumingRule);
    }

    public void DeleteVacuumingRule(VacuumingRule vacuumingRule)
    {
        _vacuumingRuleMapper.Delete(vacuumingRule);
    }

    public VacuumingRule? GetVacuumingRule(int ruleId)
    {
        return _vacuumingRuleMapper.FindSingle(vacuumingRule => vacuumingRule.Id == ruleId);
    }

    public IEnumerable<VacuumingRule> GetAllVacuumingRules()
    {
        return _vacuumingRuleMapper.Find(_ =>true);
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