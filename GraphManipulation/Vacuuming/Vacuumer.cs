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

    public IEnumerable<DeletionExecution> GenerateUpdateStatement()
    {
        List<StorageRule> allStorageRules = _purposeMapper.Find(_ => true)
            .Where(p => p.StorageRules != null)
            .SelectMany(p => p.StorageRules)
            .Where(sr => sr.PersonalDataColumn != null)
            .ToList();

        // Get all unique columns
        List<PersonalDataColumn> allColumns = allStorageRules.Select(rule => rule.PersonalDataColumn)
            .GroupBy(column => column.Key).Select(y => y.First()).ToList();

        List<DeletionExecution> deletionExecutions = new List<DeletionExecution>();
        foreach (PersonalDataColumn personalDataColumn in allColumns)
        {
            var columnRules = allStorageRules.Where(rule => rule.PersonalDataColumn.Equals(personalDataColumn));
            deletionExecutions.Add(CreateDeletionExecution(columnRules, personalDataColumn));
        }

        return deletionExecutions;
    }

    private DeletionExecution CreateDeletionExecution(IEnumerable<StorageRule> columnRules, PersonalDataColumn personalDataColumn)
    {
        var deletionExecution = new DeletionExecution();
        // Set execution purposes to the purposes of all the rules for the PersonalDataColumn
        deletionExecution.SetPurposesFromRules(columnRules);
        deletionExecution.CreateQuery(personalDataColumn, columnRules);
        deletionExecution.SetTableAndColum(personalDataColumn);
        return deletionExecution;
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
    public IEnumerable<DeletionExecution> ExecuteVacuumingRuleList(IEnumerable<VacuumingRule> vacuumingRules)
    {
        return vacuumingRules.SelectMany(ExecuteVacuumingRule);
    }

    public IEnumerable<DeletionExecution> ExecuteVacuumingRule(VacuumingRule vacuumingRule)
    {
        var executions = new List<DeletionExecution>();

        if (vacuumingRule.Purposes is null || !vacuumingRule.Purposes.Any())
        {
            _vacuumingRuleFeedbackEmitter.EmitMissing<Purpose>(vacuumingRule.Key);
            return new List<DeletionExecution>();
        }
        
        foreach (var purpose in vacuumingRule.Purposes)
        {
            if (purpose.StorageRules is null || !purpose.StorageRules.Any())
            {
                _purposeFeedbackEmitter.EmitMissing<StorageRule>(purpose.Key);
                continue;
            }

            var executionsFromStorageRules = purpose.StorageRules
                .Select(storageRule => ExecutionFromStorageRule(storageRule, vacuumingRule))
                .Where(execution => execution != null)
                .Select(execution => execution!);
                
            executions.AddRange(executionsFromStorageRules);
        }
        
        ExecuteConditions(executions);

        return executions;
    }

    private DeletionExecution? ExecutionFromStorageRule(StorageRule storageRule, VacuumingRule rule)
    {
        if (storageRule.PersonalDataColumn?.Key == null)
        {
            _storageRuleFeedbackEmitter.EmitMissing<PersonalDataColumn>(storageRule.Key);
            return null;
        }
        
        List<StorageRule> rulesWithSameTableColumn = RulesWithSameTableColumn(storageRule);

        DeletionExecution execution = CreateDeletionExecution(rulesWithSameTableColumn, storageRule.PersonalDataColumn);
        execution.VacuumingRule = rule;

        return execution;
    }

    private List<StorageRule> RulesWithSameTableColumn(StorageRule storageRule)
    {
        return _purposeMapper
            .Find(p => HasRulesForColumn(p, storageRule.PersonalDataColumn))
            .SelectMany(p => SelectRulesForColumn(p.StorageRules, storageRule.PersonalDataColumn))
            .ToList();
    }

    private static bool HasRulesForColumn(Purpose purpose, PersonalDataColumn column)
    {
        return IsValid(purpose) && purpose.StorageRules!.Any(sr => IsValid(sr) && sr.PersonalDataColumn!.Equals(column));
    }
    
    private static IEnumerable<StorageRule> SelectRulesForColumn(IEnumerable<StorageRule> rules, PersonalDataColumn column)
    {
        return rules.Where(sr => IsValid(sr) && sr.PersonalDataColumn!.Equals(column));
    }

    private static bool IsValid(StorageRule storageRule)
    {
        return storageRule.PersonalDataColumn != null && storageRule.PersonalDataColumn.Key != null;
    }

    private static bool IsValid(Purpose purpose)
    {
        return purpose.StorageRules is not null;
    }
}