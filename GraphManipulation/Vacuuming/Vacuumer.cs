using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers;
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
            output.Add(CreateDeletionExecution(
                GetAllDeleteConditionsWithSameTableColumnPair(condition.PersonalDataColumn)));
        }

        return output;
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
            foreach (var condition in purpose.DeleteConditions)
            {
                DeletionExecution execution =
                    CreateDeletionExecution(
                        GetAllDeleteConditionsWithSameTableColumnPair(condition.PersonalDataColumn));
                _queryExecutor.Execute(execution.Query);
                executions.Add(execution);
            }
        }

        return executions;
    }

    private List<DeleteCondition> GetAllDeleteConditionsWithSameTableColumnPair(PersonalDataColumn personalDataColumn)
    {
        List<Purpose> purposes = GetAllPurposesWithSameTableColumnPair(personalDataColumn.TableColumnPair);
        List<DeleteCondition> output = new List<DeleteCondition>();
        foreach (Purpose purpose in purposes)
        {
            output.AddRange(purpose.DeleteConditions);
        }

        return output;
    }

    private List<Purpose> GetAllPurposesWithSameTableColumnPair(TableColumnPair tableColumnPair)
    {
        return _purposeMapper.Find(purpose => purpose.DeleteConditions.Any(deleteCondition =>
            deleteCondition.PersonalDataColumn.TableColumnPair.Equals(tableColumnPair))).ToList();
    }

    private DeletionExecution CreateDeletionExecution(List<DeleteCondition> conditions)
    {
        DeletionExecution deletionExecution = new();
        var logicOperator = " AND ";
        string query = CreateUpdateQuery(conditions.First().PersonalDataColumn);
        foreach (var condition in conditions)
        {
            query += AppendConditions(condition, logicOperator, deletionExecution);
        }


        deletionExecution.Column = conditions.First().PersonalDataColumn.TableColumnPair.ColumnName;
        deletionExecution.Table = conditions.First().PersonalDataColumn.TableColumnPair.TableName;

        deletionExecution.Query = ReplaceLastOccurrenceOfString(query, logicOperator);
        return deletionExecution;
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