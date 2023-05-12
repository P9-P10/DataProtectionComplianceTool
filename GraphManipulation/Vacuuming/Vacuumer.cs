using GraphManipulation.DataAccess.Mappers;
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
        foreach (var condition in purpose.DeleteConditions)
        {
            AddConditionIfNotExists(output, condition);
        }

        return output;
    }

    private void AddConditionIfNotExists(List<DeletionExecution> output, DeleteCondition condition)
    {
        foreach (DeletionExecution execution in UniqueExecutions(output, condition))
        {
            output.Add(execution);
        }
    }

    private IEnumerable<DeletionExecution> UniqueExecutions(List<DeletionExecution> output, DeleteCondition condition)
    {
        return CreateDeletionExecutions(
            DeleteConditionsWithSameTableColumnPair(condition.PersonalDataColumn))
            .Where(execution => !output.Contains(execution));
    }


    private static string AppendConditions(DeleteCondition condition, string logicOperator,
        DeletionExecution deletionExecution)
    {
        string conditionalStatement = CreateConditionalStatement(condition, logicOperator);
        deletionExecution.AddPurpose(condition.Purpose);
        
        return conditionalStatement;
    }

    private static string CreateConditionalStatement(DeleteCondition condition, string logicOperator)
    {
        string conditionalStatement = $"({condition.Condition})";
        conditionalStatement += logicOperator;
        return conditionalStatement;
    }

    private static string CreateUpdateQuery(PersonalDataColumn personalDataColumn)
    {
        return
            $"UPDATE {personalDataColumn.TableColumnPair.TableName} SET {personalDataColumn.TableColumnPair.ColumnName} = '{personalDataColumn.DefaultValue}' WHERE ";
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

    private List<DeleteCondition> DeleteConditionsWithSameTableColumnPair(PersonalDataColumn personalDataColumn)
    {
        List<Purpose> purposes = GetAllPurposesWithSameTableColumnPair(personalDataColumn);
        List<DeleteCondition> output = new List<DeleteCondition>();
        foreach (Purpose purpose in purposes)
        {
            output.AddRange(purpose.DeleteConditions);
        }

        return output;
    }

    private List<Purpose> GetAllPurposesWithSameTableColumnPair(PersonalDataColumn personalDataColumn)
    {
        return _purposeMapper.Find(purpose => purpose.DeleteConditions.Any(deleteCondition =>
            deleteCondition.PersonalDataColumn.TableColumnPair.Equals(personalDataColumn.TableColumnPair)
            && !personalDataColumn.Purposes.Contains(deleteCondition.Purpose))).ToList();
    }

    private List<DeletionExecution> CreateDeletionExecutions(List<DeleteCondition> conditions)
    {
        List<DeletionExecution> output = new List<DeletionExecution>();
        var logicOperator = " AND ";

        foreach (DeleteCondition condition in conditions)
        {
            DeletionExecution? execution = output.Find(HasSameTableColumnPair(condition));
            if (execution != null)
            {
                UpdateExecution(execution, condition, logicOperator);
            }
            else
            {
                AddExecution(condition, logicOperator, output);
            }
        }

        CleanupStatement(output, logicOperator);

        return output;
    }

    private static Predicate<DeletionExecution> HasSameTableColumnPair(DeleteCondition condition)
    {
        return deleteExecution =>
            deleteExecution.Table == condition.PersonalDataColumn.TableColumnPair.TableName &&
            deleteExecution.Column == condition.PersonalDataColumn.TableColumnPair.ColumnName;
    }

    private static void UpdateExecution(DeletionExecution execution, DeleteCondition condition, string logicOperator)
    {
        execution.Query += AppendConditions(condition, logicOperator, execution);
    }

    private static void AddExecution(DeleteCondition condition, string logicOperator, List<DeletionExecution> output)
    {
        DeletionExecution execution = new();
        string updateQuery = CreateUpdateQuery(condition.PersonalDataColumn);

        execution.Query += updateQuery + AppendConditions(condition, logicOperator, execution);

        execution.Column = condition.PersonalDataColumn.TableColumnPair.ColumnName;
        execution.Table = condition.PersonalDataColumn.TableColumnPair.TableName;
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