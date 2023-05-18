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
        List<StorageRule> allStorageRules = _purposeMapper.Find(_ => true)
            .Where(p => p.StorageRules != null)
            .SelectMany(p => p.StorageRules)
            .Where(sr => sr.PersonalDataColumn != null)
            .ToList();

        // Get all unique columns
        // Gets them via StorageRules
        // Could be done more directly using a Mapper for PersonalDataColumn
        List<PersonalDataColumn> allColumns = allStorageRules.Select(rule => rule.PersonalDataColumn)
            .GroupBy(column => column.Key).Select(y => y.First()).ToList();

        List<DeletionExecution> deletionExecutions = new List<DeletionExecution>();
        foreach (PersonalDataColumn personalDataColumn in allColumns)
        {
            var deletionExecution = new DeletionExecution();

            var columnRules = allStorageRules.Where(rule => rule.PersonalDataColumn.Equals(personalDataColumn));

            // Set execution purposes to the purposes of all the rules for the PersonalDataColumn
            deletionExecution.SetPurposesFromRules(columnRules);

            // This is the more direct way of adding purposes
            // But unit tests do not define PersonalDataColumns for purposes

            // Set execution purposes to all purposes for the PersonalDataColumn
            //deletionExecution.Purposes = allPurposes
            //    .Where(purpose => purpose.PersonalDataColumns
            //        .Contains(personalDataColumn))
            //    .ToList();

            deletionExecution.CreateQuery(personalDataColumn, columnRules);

            deletionExecution.SetTableAndColum(personalDataColumn);

            deletionExecutions.Add(deletionExecution);
        }

        return deletionExecutions;
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

        foreach (VacuumingRule rule in vacuumingRules)
        {
            if (rule.Purposes is null || !rule.Purposes.Any())
            {
                _vacuumingRuleFeedbackEmitter.EmitMissing<Purpose>(rule.Key);
                return executions;
            }

            foreach (var purpose in rule.Purposes)
            {
                if (purpose.StorageRules is null || !purpose.StorageRules.Any())
                {
                    _purposeFeedbackEmitter.EmitMissing<StorageRule>(purpose.Key);
                    return executions;
                }

                executions.AddRange(purpose.StorageRules.Select(storageRule =>
                    ExecutionFromStorageRule(storageRule, purpose, rule)));
            }

            CleanupStatement(executions, " AND");
            ExecuteConditions(executions);
            return executions;
        }

        return executions;
    }

    private DeletionExecution ExecutionFromStorageRule(StorageRule storageRule, Purpose purpose, VacuumingRule rule)
    {
        DeletionExecution execution = new();
        if (storageRule.PersonalDataColumn == null || storageRule.PersonalDataColumn.Key == null)
        {
            return null;
        }

        execution.Query = CreateUpdateQuery(storageRule.PersonalDataColumn);
        List<StorageRule> rulesWithSameTableColumn = RulesWithSameTableColumn(storageRule);

        execution.CreateQuery(storageRule.PersonalDataColumn, rulesWithSameTableColumn);
        execution.SetTableAndColum(storageRule.PersonalDataColumn);
        execution.AddPurpose(purpose);
        execution.VacuumingRule = rule;
        execution.SetPurposesFromRules(rulesWithSameTableColumn);
        return execution;
    }

    private List<StorageRule> RulesWithSameTableColumn(StorageRule storageRule)
    {
        return _purposeMapper.Find(p =>
                p.StorageRules != null && p.StorageRules.Any(s =>
                    s.PersonalDataColumn is {Key: not null} &&
                    s.PersonalDataColumn.Key.Equals(storageRule.PersonalDataColumn.Key)))
            .SelectMany(HasSameTableColumn(storageRule)).ToList();
    }

    private static Func<Purpose, IEnumerable<StorageRule>> HasSameTableColumn(StorageRule storageRule)
    {
        return p =>
        {
            if (p.StorageRules != null)
                return p.StorageRules.Where(
                    sr => storageRule.PersonalDataColumn is {Key: not null} && sr.PersonalDataColumn != null &&
                          storageRule.PersonalDataColumn != null &&
                          sr.PersonalDataColumn.Key != null && sr.PersonalDataColumn != null &&
                          sr.PersonalDataColumn.Key.Equals(storageRule.PersonalDataColumn.Key));
            return new List<StorageRule>();
        };
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