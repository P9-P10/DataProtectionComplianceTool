using Dapper;
using FluentAssertions;
using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class VacuumingPoliciesTest : TestResources
{
    [Fact]
    public void VacuumingPoliciesCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddVacuumingPolicy(process, VacuumingPolicy);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<string, VacuumingPolicy>(VacuumingPolicy.Key!,
                SystemOperation.Operation.Created, null));
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(VacuumingPolicy.Key!,
                SystemOperation.Operation.Updated, VacuumingPolicy));
    }

    [Fact]
    public void VacuumingPoliciesCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddVacuumingPolicy(process, VacuumingPolicy);
        ShowVacuumingPolicy(process, VacuumingPolicy);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(VacuumingPolicy.ToListing()));
    }

    [Fact]
    public void VacuumingPoliciesCanBeUpdated()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddVacuumingPolicy(process, VacuumingPolicy);
        UpdateVacuumingPolicy(process, VacuumingPolicy, UpdatedTestVacuumingPolicy);
        ShowVacuumingPolicy(process, UpdatedTestVacuumingPolicy);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(UpdatedTestVacuumingPolicy.ToListing()));
    }

    [Fact]
    public void VacuumingPoliciesCanBeListed()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddVacuumingPolicy(process, VacuumingPolicy);
        AddVacuumingPolicy(process, UpdatedTestVacuumingPolicy);
        ListVacuumingPolicy(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(VacuumingPolicy.ToListing()));
        output.Should().ContainSingle(s => s.Contains(UpdatedTestVacuumingPolicy.ToListing()));
    }

    [Fact]
    public void VacuumingPoliciesCanBeDeleted()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddVacuumingPolicy(process, VacuumingPolicy);
        DeleteVacuumingPolicy(process, VacuumingPolicy);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains($"Vacuuming policy '{VacuumingPolicy.ToListingIdentifier()}' successfully deleted"));
    }

    [Fact]
    public void VacuumingPoliciesCanReceivePurposes()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddStoragePolicy(process, NewTestStoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddVacuumingPolicy(process, VacuumingPolicy);
        AddPurposesToVacuumingPolicy(process, VacuumingPolicy, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"Vacuuming policy '{VacuumingPolicy.ToListingIdentifier()}' successfully updated")
            && s.Contains(NewTestPurpose.ToListingIdentifier())
            && s.Contains(VeryNewTestPurpose.ToListingIdentifier()));
    }

    [Fact]
    public void VacuumingPoliciesCanHavePurposesRemoved()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddStoragePolicy(process, NewTestStoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddVacuumingPolicy(process, VacuumingPolicy);
        AddPurposesToVacuumingPolicy(process, VacuumingPolicy, new[] { NewTestPurpose, VeryNewTestPurpose });
        RemovePurposesFromVacuumingPolicy(process, VacuumingPolicy, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"Vacuuming policy '{VacuumingPolicy.ToListingIdentifier()}' successfully updated")
            && !s.Contains(NewTestPurpose.ToListingIdentifier())
            && !s.Contains(VeryNewTestPurpose.ToListingIdentifier()));
    }

    [Fact]
    public void VacuumingPoliciesCanBeExecuted()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection, process);

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        UpdateStoragePolicyWithPersonalDataColumn(process, StoragePolicy, TestPersonalDataColumn);
        AddVacuumingPolicy(process, VacuumingPolicy);

        ReportStatus(process);

        var statusOutput = process.GetLastOutputNoWhitespaceOrPrompt();
        statusOutput.Where(s => !s.Contains("Individual")).Should().BeEmpty();

        ExecuteVacuumingPolicy(process, new[] { VacuumingPolicy });

        // Operation made such that the ExecuteVacuumingPolicy has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespaceOrPrompt();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.ResultMessage<string, VacuumingPolicy>(VacuumingPolicy.Key!,
                SystemOperation.Operation.Executed, null, null));
    }

    [Fact]
    public void ExecutingVacuumingPoliciesHandlesNullReferencesGracefullyStoragePolicyMissingPersonalDataColumn()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection, process);

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        // UpdateStoragePolicyWithPersonalDataColumn(process, StoragePolicy, TestPersonalDataColumn);
        AddVacuumingPolicy(process, VacuumingPolicy);
        ExecuteVacuumingPolicy(process, new[] { VacuumingPolicy });

        // Operation made such that the ExecuteVacuumingPolicy has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        error.Should().BeEmpty();
    }

    [Fact]
    public void ExecutingVacuumingPoliciesEmitsStoragePolicyMissingPersonalDataColumn()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection, process);

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        // UpdateStoragePolicyWithPersonalDataColumn(process, StoragePolicy, TestPersonalDataColumn);
        AddVacuumingPolicy(process, VacuumingPolicy);
        ExecuteVacuumingPolicy(process, new[] { VacuumingPolicy });

        var output = process.GetLastOutputNoWhitespaceOrPrompt();
        output.Should()
            .Contain(
                FeedbackEmitterMessage.MissingMessage<string, StoragePolicy, PersonalDataColumn>(StoragePolicy.Key));

        // Operation made such that the ExecuteVacuumingPolicy has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        error.Should().BeEmpty();
    }

    [Fact]
    public void ExecutingVacuumingPoliciesHandlesNullReferencesGracefullyPurposeMissingStoragePolicy()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection, process);

        AddStoragePolicy(process, StoragePolicy);
        // AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        UpdateStoragePolicyWithPersonalDataColumn(process, StoragePolicy, TestPersonalDataColumn);
        AddVacuumingPolicy(process, VacuumingPolicy);
        ExecuteVacuumingPolicy(process, new[] { VacuumingPolicy });

        // Operation made such that the ExecuteVacuumingPolicy has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        error.Should().BeEmpty();
    }

    [Fact]
    public void ExecutingVacuumingPoliciesEmitsPurposeMissingStoragePolicy()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection, process);

        AddStoragePolicy(process, StoragePolicy);
        // AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        UpdateStoragePolicyWithPersonalDataColumn(process, StoragePolicy, TestPersonalDataColumn);
        AddVacuumingPolicy(process, VacuumingPolicy);
        ExecuteVacuumingPolicy(process, new[] { VacuumingPolicy });

        var output = process.GetLastOutputNoWhitespaceOrPrompt();
        output.Should()
            .Contain(
                FeedbackEmitterMessage.MissingMessage<string, Purpose, StoragePolicy>(TestPurpose.Key));

        // Operation made such that the ExecuteVacuumingPolicy has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        error.Should().BeEmpty();
    }

    [Fact]
    public void ExecutingVacuumingPoliciesAffectsData()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        StoragePolicy.VacuumingCondition = "Id = 2";

        SetupTestData(dbConnection, process);

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        UpdateStoragePolicyWithPersonalDataColumn(process, StoragePolicy, TestPersonalDataColumn);
        AddVacuumingPolicy(process, VacuumingPolicy);

        // Status should be empty, except regarding individuals missing origins
        ReportStatus(process);

        var statusOutput = process.GetLastOutputNoWhitespaceOrPrompt();
        statusOutput.Where(s => !s.Contains("Individual")).Should().BeEmpty();

        // Console should show that the vacuuming policy has been executed
        ExecuteVacuumingPolicy(process, new[] { VacuumingPolicy });

        process.GetAllErrorsNoWhitespace().Should().BeEmpty();

        var executeOutput = process.GetLastOutputNoWhitespaceOrPrompt().ToList();
        executeOutput.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.ResultMessage<string, VacuumingPolicy>(VacuumingPolicy.Key!,
                SystemOperation.Operation.Executed, null, null));
        executeOutput.Should().NotContain(s => s.Contains("had no effect"));

        // The log should contain entries regarding the vacuuming policies that have been executed
        ListLogs(process, new LogConstraints(logTypes: new[] { LogType.Vacuuming }));

        var logOutput = process.GetLastOutputNoWhitespaceOrPrompt().ToList();
        logOutput.Should().Contain(s => s.Contains(FeedbackEmitterMessage.ResultMessage<string, VacuumingPolicy>(
            VacuumingPolicy.Key!,
            SystemOperation.Operation.Executed, null, null)));
        logOutput.Should().Contain(s =>
            s.Contains("WHERE") &&
            s.Contains(StoragePolicy.VacuumingCondition) &&
            s.Contains("affected") &&
            s.Contains(TestPersonalDataColumn.ToListingIdentifier()));

        // There should be no errors
        process.GetAllErrorsNoWhitespace().Should().BeEmpty();

        var result = dbConnection.Query<(string key, string column)>(
                $"SELECT Id, {TestPersonalDataColumn.Key.ColumnName} " +
                $"FROM {TestPersonalDataColumn.Key.TableName}")
            .ToList();

        // The vacuuming condition should only affect one of the three rows, changing its value to the default value
        result.First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual1.ToListingIdentifier(),
                TestIndividual1.ToListingIdentifier()));
        result.Skip(1).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual2.ToListingIdentifier(),
                TestPersonalDataColumn.DefaultValue));
        result.Skip(2).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual3.ToListingIdentifier(),
                TestIndividual3.ToListingIdentifier()));
    }

    [Fact]
    public void ExecutingVacuumingPoliciesWithNoHitsDoesNotAffectData()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        StoragePolicy.VacuumingCondition = "FALSE";

        SetupTestData(dbConnection, process);

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        UpdateStoragePolicyWithPersonalDataColumn(process, StoragePolicy, TestPersonalDataColumn);
        AddVacuumingPolicy(process, VacuumingPolicy);

        ExecuteVacuumingPolicy(process, new[] { VacuumingPolicy });

        // Operation made such that the ExecuteVacuumingPolicy has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        process.GetAllErrorsNoWhitespace().Should().BeEmpty();

        var result = dbConnection.Query<(string key, string column)>(
                $"SELECT Id, {TestPersonalDataColumn.Key.ColumnName} " +
                $"FROM {TestPersonalDataColumn.Key.TableName}")
            .ToList();

        result.First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual1.ToListingIdentifier(),
                TestIndividual1.ToListingIdentifier()));
        result.Skip(1).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual2.ToListingIdentifier(),
                TestIndividual2.ToListingIdentifier()));
        result.Skip(2).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual3.ToListingIdentifier(),
                TestIndividual3.ToListingIdentifier()));
    }
}