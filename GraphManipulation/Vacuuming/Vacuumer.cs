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
            output.AddRange(CreateDeletionExecutionFromPurpose(purpose));
        }

        return output;
    }

    private IEnumerable<DeletionExecution> CreateDeletionExecutionFromPurpose(Purpose purpose)
    {
        List<DeletionExecution> output = new List<DeletionExecution>();
        foreach (var condition in purpose.DeleteConditions)
        {
            AddIfNotExists(output, condition);
        }


        return output;
    }

    private void AddIfNotExists(List<DeletionExecution> output, DeleteCondition condition)
    {
        foreach (var execution in CreateDeletionExecutions(
                     DeleteConditionsWithSameTableColumnPair(condition.PersonalDataColumn)))
        {
            if (output.Contains(execution)) continue;
            output.Add(execution);
        }
    }


    private static string AppendConditions(DeleteCondition condition, string logicOperator,
        DeletionExecution deletionExecution)
    {
        string conditionalStatement = "";
        conditionalStatement += $"({condition.Condition})";
        conditionalStatement += logicOperator;
        deletionExecution.AddPurpose(condition.Purpose);


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

    private List<DeletionExecution> ExecuteVacuumingRule(VacuumingRule vacuumingRule)
    {
        List<DeletionExecution> executions = new List<DeletionExecution>();

        foreach (var purpose in vacuumingRule.Purposes)
        {
            executions.AddRange(CreateDeletionExecutionFromPurpose(purpose));
        }
        ExecuteConditions(executions);
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
            DeletionExecution? execution = output.Find(deleteExecution =>
                deleteExecution.Table == condition.PersonalDataColumn.TableColumnPair.TableName &&
                deleteExecution.Column == condition.PersonalDataColumn.TableColumnPair.ColumnName);
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

    private static void UpdateExecution(DeletionExecution execution, DeleteCondition condition, string logicOperator)
    {
        execution.Query += AppendConditions(condition, logicOperator, execution);
    }

    private static void AddExecution(DeleteCondition condition, string logicOperator, List<DeletionExecution> output)
    {
        DeletionExecution execution;
        execution = new DeletionExecution();
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