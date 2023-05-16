using GraphManipulation.DataAccess;
using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly IMapper<Purpose> _purposeMapper;
    private readonly IQueryExecutor _queryExecutor;


    public Vacuumer(IMapper<Purpose> purposeMapper, IQueryExecutor queryExecutor)
    {
        _purposeMapper = purposeMapper;
        _queryExecutor = queryExecutor;
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
        foreach (var condition in purpose.StorageRules)
        {
            AddConditionIfNotExists(output, condition, purpose);
        }

        return output;
    }

    private void AddConditionIfNotExists(List<DeletionExecution> output, StorageRule condition, Purpose purpose)
    {
        foreach (DeletionExecution execution in UniqueExecutions(output, condition, purpose))
        {
            output.Add(execution);
        }
    }

    private IEnumerable<DeletionExecution> UniqueExecutions(List<DeletionExecution> output, StorageRule condition,
        Purpose purpose)
    {
        return CreateDeletionExecutions(
                DeleteConditionsWithSameTableColumnPair(condition.PersonalDataColumn), purpose)
            .Where(execution => !output.Contains(execution));
    }


    private static string AppendConditions(StorageRule condition, string logicOperator,
        DeletionExecution deletionExecution, Purpose purpose)
    {
        string conditionalStatement = CreateConditionalStatement(condition, logicOperator);
        deletionExecution.AddPurpose(purpose);

        return conditionalStatement;
    }

    private static string CreateConditionalStatement(StorageRule condition, string logicOperator)
    {
        string conditionalStatement = $"({condition.VacuumingCondition})";
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

    private List<DeletionExecution> CreateDeletionExecutions(List<StorageRule> conditions, Purpose purpose)
    {
        List<DeletionExecution> output = new List<DeletionExecution>();
        var logicOperator = " AND ";

        foreach (StorageRule condition in conditions)
        {
            DeletionExecution? execution = output.Find(HasSameTableColumnPair(condition));
            if (execution != null)
            {
                UpdateExecution(execution, condition, logicOperator, purpose);
            }
            else
            {
                AddExecution(condition, logicOperator, output, purpose);
            }
        }

        CleanupStatement(output, logicOperator);

        return output;
    }

    private static Predicate<DeletionExecution> HasSameTableColumnPair(StorageRule condition)
    {
        return deleteExecution =>
            deleteExecution.Table == condition.PersonalDataColumn.Key.TableName &&
            deleteExecution.Column == condition.PersonalDataColumn.Key.ColumnName;
    }

    private static void UpdateExecution(DeletionExecution execution, StorageRule condition, string logicOperator,
        Purpose purpose)
    {
        execution.Query += AppendConditions(condition, logicOperator, execution, purpose);
    }

    private static void AddExecution(StorageRule condition, string logicOperator, List<DeletionExecution> output,
        Purpose purpose)
    {
        DeletionExecution execution = new();
        string updateQuery = CreateUpdateQuery(condition.PersonalDataColumn);

        execution.Query += updateQuery + AppendConditions(condition, logicOperator, execution, purpose);

        execution.Column = condition.PersonalDataColumn.Key.ColumnName;
        execution.Table = condition.PersonalDataColumn.Key.TableName;
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