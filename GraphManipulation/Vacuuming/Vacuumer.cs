using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Models;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly IMapper<PersonalDataColumn> _personalDataColumnMapper;
    private readonly IQueryExecutor _queryExecutor;

    public Vacuumer(IMapper<PersonalDataColumn> personalDataColumnMapper, IQueryExecutor queryExecutor)
    {
        _personalDataColumnMapper = personalDataColumnMapper;
        _queryExecutor = queryExecutor;
    }

    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        return _personalDataColumnMapper.Find(_ => true).Select(CreateDeletionExecution).ToList();
    }

    private static DeletionExecution CreateDeletionExecution(PersonalDataColumn personalDataColumn)
    {
        DeletionExecution deletionExecution = new();
        var logicOperator = " AND ";
        string query = CreateUpdateQuery(personalDataColumn);

        query += AppendConditions(personalDataColumn, logicOperator, deletionExecution);

        deletionExecution.Column = personalDataColumn.TableColumnPair.ColumnName;
        deletionExecution.Table = personalDataColumn.TableColumnPair.TableName;

        deletionExecution.Query = ReplaceLastOccurrenceOfString(query, logicOperator);
        return deletionExecution;
    }

    private static string AppendConditions(PersonalDataColumn personalDataColumn, string logicOperator,
        DeletionExecution deletionExecution)
    {
        string conditionalStatement = "";
        foreach (Purpose purpose in personalDataColumn.Purposes)
        {
            foreach (var condition in purpose.DeleteConditions)
            {
                conditionalStatement += $"({condition})";
                conditionalStatement += logicOperator;
                deletionExecution.AddPurpose(purpose);
            }
        }

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

        List<PersonalDataColumn> personalDataColumns = _personalDataColumnMapper.Find(x => true).ToList();

        foreach (var personalDataColumn in personalDataColumns)
        {
            foreach (var rule in vacuumingRules.ToList())
            {
                bool containsCorrectCondition = ContainsCorrectCondition(personalDataColumn, rule);
                if (!containsCorrectCondition) continue;

                DeletionExecution execution = CreateDeletionExecution(personalDataColumn);
                executions.Add(execution);
                _queryExecutor.Execute(execution.Query);
            }
        }

        return executions;
    }

    private static bool ContainsCorrectCondition(PersonalDataColumn personalDataColumn, VacuumingRule? rule)
    {
        foreach (var purpose in personalDataColumn.Purposes)
        {
            if (rule != null && rule.Purposes.Contains(purpose))
            {
                return true;
            }
        }

        return false;
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