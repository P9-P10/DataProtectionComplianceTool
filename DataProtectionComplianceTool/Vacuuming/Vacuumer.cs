using GraphManipulation.DataAccess;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly FeedbackEmitter<string, Purpose> _purposeFeedbackEmitter;
    private readonly IMapper<Purpose> _purposeMapper;
    private readonly IQueryExecutor _queryExecutor;
    private readonly FeedbackEmitter<string, StoragePolicy> _storagePolicyFeedbackEmitter;
    private readonly FeedbackEmitter<string, VacuumingPolicy> _vacuumingPolicyFeedbackEmitter;


    public Vacuumer(IMapper<Purpose> purposeMapper, IQueryExecutor queryExecutor)
    {
        _purposeMapper = purposeMapper;
        _queryExecutor = queryExecutor;
        _vacuumingPolicyFeedbackEmitter = new FeedbackEmitter<string, VacuumingPolicy>();
        _purposeFeedbackEmitter = new FeedbackEmitter<string, Purpose>();
        _storagePolicyFeedbackEmitter = new FeedbackEmitter<string, StoragePolicy>();
    }

    public IEnumerable<DeletionExecution> GenerateUpdateStatement()
    {
        List<StoragePolicy> allStoragePolicies = _purposeMapper.Find(_ => true)
            .Where(p => p.StoragePolicies != null)
            .SelectMany(p => p.StoragePolicies)
            .Where(sr => sr.PersonalDataColumn != null)
            .ToList();

        // Get all unique columns
        List<PersonalDataColumn> allColumns = allStoragePolicies.Select(storagePolicy => storagePolicy.PersonalDataColumn)
            .GroupBy(column => column.Key).Select(y => y.First()).ToList();

        List<DeletionExecution> deletionExecutions = new List<DeletionExecution>();
        foreach (PersonalDataColumn personalDataColumn in allColumns)
        {
            var columnStoragePolicies = allStoragePolicies.Where(storagePolicy => storagePolicy.PersonalDataColumn.Equals(personalDataColumn));
            deletionExecutions.Add(CreateDeletionExecution(columnStoragePolicies, personalDataColumn));
        }

        return deletionExecutions;
    }

    public IEnumerable<DeletionExecution> Execute()
    {
        IEnumerable<DeletionExecution> executions = GenerateUpdateStatement();
        var deletionExecutions = executions.ToList();
        ExecuteConditions(deletionExecutions);

        return deletionExecutions;
    }

    /// <summary>
    ///     This function executes a specified vacuuming policy.
    ///     It does not vacuum data if its protected by other purposes.
    /// </summary>
    /// <param name="vacuumingPolicies"></param>
    /// <returns></returns>
    public IEnumerable<DeletionExecution> ExecuteVacuumingPolicyList(IEnumerable<VacuumingPolicy> vacuumingPolicies)
    {
        return vacuumingPolicies.SelectMany(ExecuteVacuumingPolicy);
    }

    public IEnumerable<DeletionExecution> ExecuteVacuumingPolicy(VacuumingPolicy vacuumingPolicy)
    {
        var executions = new List<DeletionExecution>();

        if (vacuumingPolicy.Purposes is null || !vacuumingPolicy.Purposes.Any())
        {
            _vacuumingPolicyFeedbackEmitter.EmitMissing<Purpose>(vacuumingPolicy.Key);
            return new List<DeletionExecution>();
        }

        foreach (var purpose in vacuumingPolicy.Purposes)
        {
            if (purpose.StoragePolicies is null || !purpose.StoragePolicies.Any())
            {
                _purposeFeedbackEmitter.EmitMissing<StoragePolicy>(purpose.Key);
                continue;
            }

            var executionsFromStoragePolicies = purpose.StoragePolicies
                .Select(storagePolicy => ExecutionFromStoragePolicy(storagePolicy, vacuumingPolicy))
                .Where(execution => execution != null)
                .Select(execution => execution!);

            executions.AddRange(executionsFromStoragePolicies);
        }

        ExecuteConditions(executions);

        return executions;
    }

    private static DeletionExecution CreateDeletionExecution(IEnumerable<StoragePolicy> columnStoragePolicies,
        PersonalDataColumn personalDataColumn)
    {
        var deletionExecution = new DeletionExecution();
        // Set execution purposes to the purposes of all the policies for the PersonalDataColumn
        deletionExecution.SetPurposesFromStoragePolicies(columnStoragePolicies);
        deletionExecution.CreateQuery(personalDataColumn, columnStoragePolicies);
        deletionExecution.SetTableAndColum(personalDataColumn);
        return deletionExecution;
    }

    private void ExecuteConditions(List<DeletionExecution> deletionExecutions)
    {
        foreach (var deletionExecution in deletionExecutions)
            _queryExecutor.Execute(deletionExecution.Query);
    }

    private DeletionExecution? ExecutionFromStoragePolicy(StoragePolicy storagePolicy, VacuumingPolicy policy)
    {
        if (storagePolicy.PersonalDataColumn?.Key == null)
        {
            _storagePolicyFeedbackEmitter.EmitMissing<PersonalDataColumn>(storagePolicy.Key);
            return null;
        }

        List<StoragePolicy> policiesWithSameTableColumn = StoragePoliciesWithSameTableColumn(storagePolicy);

        DeletionExecution execution = CreateDeletionExecution(policiesWithSameTableColumn, storagePolicy.PersonalDataColumn);
        execution.VacuumingPolicy = policy;

        return execution;
    }

    private List<StoragePolicy> StoragePoliciesWithSameTableColumn(StoragePolicy storagePolicy)
    {
        return _purposeMapper
            .Find(p => HasStoragePoliciesForColumn(p, storagePolicy.PersonalDataColumn))
            .SelectMany(p => SelectStoragePoliciesForColumn(p.StoragePolicies, storagePolicy.PersonalDataColumn))
            .ToList();
    }

    private static bool HasStoragePoliciesForColumn(Purpose purpose, PersonalDataColumn column)
    {
        return IsValid(purpose) &&
               purpose.StoragePolicies!.Any(sr => IsValid(sr) && sr.PersonalDataColumn!.Equals(column));
    }

    private static IEnumerable<StoragePolicy> SelectStoragePoliciesForColumn(IEnumerable<StoragePolicy> storagePolicies,
        PersonalDataColumn column)
    {
        return storagePolicies.Where(sr => IsValid(sr) && sr.PersonalDataColumn!.Equals(column));
    }

    private static bool IsValid(StoragePolicy storagePolicy)
    {
        return storagePolicy.PersonalDataColumn != null && storagePolicy.PersonalDataColumn.Key != null;
    }

    private static bool IsValid(Purpose purpose)
    {
        return purpose.StoragePolicies is not null;
    }
}