using GraphManipulation.DataAccess;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly IMapper<Purpose> _purposeMapper;
    private readonly IQueryExecutor _queryExecutor;
    private readonly FeedbackEmitter<string, VacuumingRule> _vacuumingRuleFeedbackEmitter;
    private readonly FeedbackEmitter<string, Purpose> _purposeFeedbackEmitter;
    private readonly FeedbackEmitter<string, StorageRule> _storageRuleFeedbackEmitter;


    public Vacuumer(IMapper<Purpose> purposeMapper, IQueryExecutor queryExecutor)
    {
        _purposeMapper = purposeMapper;
        _queryExecutor = queryExecutor;
        _vacuumingRuleFeedbackEmitter = new FeedbackEmitter<string, VacuumingRule>();
        _purposeFeedbackEmitter = new FeedbackEmitter<string, Purpose>();
        _storageRuleFeedbackEmitter = new FeedbackEmitter<string, StorageRule>();
    }

    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        List<DeletionExecution> output = new List<DeletionExecution>();
        foreach (var purpose in _purposeMapper.Find(_ => true))
        {
            output.AddRange(CreateExecutionsFromPurpose(purpose));
        }

        return output;
    }


    private List<DeletionExecution> CreateExecutionsFromPurpose(Purpose purpose)
    {
        List<DeletionExecution> output = new List<DeletionExecution>();

        if (purpose.StorageRules is null || !purpose.StorageRules.Any())
        {
            _purposeFeedbackEmitter.EmitMissing<StorageRule>(purpose.Key);
            return output;
        }
        
        foreach (var storageRule in purpose.StorageRules)
        {
            AddConditionIfNotExists(output, storageRule, purpose);
        }

        return output;
    }

    private void AddConditionIfNotExists(List<DeletionExecution> output, StorageRule storageRule, Purpose purpose)
    {
        var uniqueExecutions = UniqueExecutions(output, storageRule, purpose);
        output.AddRange(uniqueExecutions);
    }

    private IEnumerable<DeletionExecution> UniqueExecutions(List<DeletionExecution> output, StorageRule storageRule,
        Purpose purpose)
    {
        if (storageRule.PersonalDataColumn is null)
        {
            _storageRuleFeedbackEmitter.EmitMissing<PersonalDataColumn>(storageRule.Key);
            return new List<DeletionExecution>();
        }
        
        return CreateDeletionExecutions(
                DeleteConditionsWithSameTableColumnPair(storageRule.PersonalDataColumn), purpose)
            .Where(execution => !output.Contains(execution));
    }


    private static string AppendConditions(StorageRule storageRule, string logicOperator,
        DeletionExecution deletionExecution, Purpose purpose)
    {
        string conditionalStatement = CreateConditionalStatement(storageRule, logicOperator);
        deletionExecution.AddPurpose(purpose);

        return conditionalStatement;
    }

    private static string CreateConditionalStatement(StorageRule storageRule, string logicOperator)
    {
        string conditionalStatement = $"({storageRule.VacuumingCondition})";
        conditionalStatement += logicOperator;
        return conditionalStatement;
    }

    private static string CreateUpdateQuery(PersonalDataColumn personalDataColumn)
    {
        return
            $"UPDATE {personalDataColumn.Key.TableName} SET {personalDataColumn.Key.ColumnName} = '{personalDataColumn.DefaultValue}' WHERE ";
    }

    public IEnumerable<DeletionExecution> Execute()
    {
        IEnumerable<DeletionExecution> executions = GenerateUpdateStatement();
        var deletionExecutions = executions.ToList();
        ExecuteConditions(deletionExecutions);

        return deletionExecutions;
    }

    private void ExecuteConditions(List<DeletionExecution> deletionExecutions)
    {
        foreach (var deletionExecution in deletionExecutions)
        {
            _queryExecutor.Execute(deletionExecution.Query);
        }
    }

    /// <summary>
    /// This function executes a specified vacuuming rule.
    /// It does not vacuum data if its protected by other purposes.
    /// </summary>
    /// <param name="vacuumingRules"></param>
    /// <returns></returns>
    public IEnumerable<DeletionExecution> ExecuteVacuumingRules(IEnumerable<VacuumingRule> vacuumingRules)
    {
        List<DeletionExecution> executions = new List<DeletionExecution>();

        foreach (var rule in vacuumingRules)
        {
            executions.AddRange(ExecuteVacuumingRule(rule));
        }

        return executions;
    }

    private IEnumerable<DeletionExecution> ExecuteVacuumingRule(VacuumingRule vacuumingRule)
    {
        var executions = CreateExecutionsFromRule(vacuumingRule);
        ExecuteConditions(executions);
        return executions;
    }

    private List<DeletionExecution> CreateExecutionsFromRule(VacuumingRule vacuumingRule)
    {
        List<DeletionExecution> executions = new List<DeletionExecution>();
        
        if (vacuumingRule.Purposes is null || !vacuumingRule.Purposes.Any())
        {
            _vacuumingRuleFeedbackEmitter.EmitMissing<Purpose>(vacuumingRule.Key);
            return executions;
        }
        
        foreach (var purpose in vacuumingRule.Purposes)
        {
            executions.AddRange(CreateExecutionsFromPurpose(purpose));
        }

        return executions;
    }

    private List<StorageRule> DeleteConditionsWithSameTableColumnPair(PersonalDataColumn personalDataColumn)
    {
        List<Purpose> purposes = GetAllPurposesWithSameTableColumnPair(personalDataColumn);
        List<StorageRule> output = new List<StorageRule>();
        foreach (Purpose purpose in purposes)
        {
            output.AddRange(purpose.StorageRules);
        }

        return output;
    }

    private List<Purpose> GetAllPurposesWithSameTableColumnPair(PersonalDataColumn personalDataColumn)
    {
        return _purposeMapper.Find(purpose => purpose.StorageRules.Any(storageRule =>
            storageRule.PersonalDataColumn.Key.Equals(personalDataColumn.Key)
            && !personalDataColumn.Purposes.Contains(purpose))).ToList();
    }

    private List<DeletionExecution> CreateDeletionExecutions(List<StorageRule> storageRules, Purpose purpose)
    {
        List<DeletionExecution> output = new List<DeletionExecution>();
        var logicOperator = " AND ";

        foreach (StorageRule storageRule in storageRules)
        {
            DeletionExecution? execution = output.Find(HasSameTableColumnPair(storageRule));
            if (execution != null)
            {
                UpdateExecution(execution, storageRule, logicOperator, purpose);
            }
            else
            {
                AddExecution(storageRule, logicOperator, output, purpose);
            }
        }

        CleanupStatement(output, logicOperator);

        return output;
    }

    private static Predicate<DeletionExecution> HasSameTableColumnPair(StorageRule storageRule)
    {
        return deleteExecution =>
            deleteExecution.Table == storageRule.PersonalDataColumn.Key.TableName &&
            deleteExecution.Column == storageRule.PersonalDataColumn.Key.ColumnName;
    }

    private static void UpdateExecution(DeletionExecution execution, StorageRule storageRule, string logicOperator,
        Purpose purpose)
    {
        execution.Query += AppendConditions(storageRule, logicOperator, execution, purpose);
    }

    private static void AddExecution(StorageRule storageRule, string logicOperator, List<DeletionExecution> output,
        Purpose purpose)
    {
        DeletionExecution execution = new();
        string updateQuery = CreateUpdateQuery(storageRule.PersonalDataColumn);

        execution.Query += updateQuery + AppendConditions(storageRule, logicOperator, execution, purpose);

        execution.Column = storageRule.PersonalDataColumn.Key.ColumnName;
        execution.Table = storageRule.PersonalDataColumn.Key.TableName;
        output.Add(execution);
    }

    private static void CleanupStatement(List<DeletionExecution> output, string logicOperator)
    {
        foreach (var execution in output)
        {
            execution.Query = ReplaceLastOccurrenceOfString(execution.Query, logicOperator);
        }
    }

    private static string ReplaceLastOccurrenceOfString(string inputString, string occurrenceToReplace,
        string replaceWith = ";")
    {
        var place = inputString.LastIndexOf(occurrenceToReplace, StringComparison.Ordinal);

        return place == -1
            ? inputString
            : inputString.Remove(place, occurrenceToReplace.Length).Insert(place, replaceWith);
    }
}