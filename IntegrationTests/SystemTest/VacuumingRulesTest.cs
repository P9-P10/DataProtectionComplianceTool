using Dapper;
using FluentAssertions;
using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class VacuumingRulesTest : TestResources
{
    [Fact]
    public void VacuumingRulesCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<string, VacuumingRule>(TestVacuumingRule.Key!,
                SystemOperation.Operation.Created, null));
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestVacuumingRule.Key!,
                SystemOperation.Operation.Updated, TestVacuumingRule));
    }

    [Fact]
    public void VacuumingRulesCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        ShowVacuumingRule(process, TestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(TestVacuumingRule.ToListing()));
    }

    [Fact]
    public void VacuumingRulesCanBeUpdated()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        UpdateVacuumingRule(process, TestVacuumingRule, UpdatedTestVacuumingRule);
        ShowVacuumingRule(process, UpdatedTestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(UpdatedTestVacuumingRule.ToListing()));
    }

    [Fact]
    public void VacuumingRulesCanBeListed()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        AddVacuumingRule(process, UpdatedTestVacuumingRule);
        ListVacuumingRule(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(TestVacuumingRule.ToListing()));
        output.Should().ContainSingle(s => s.Contains(UpdatedTestVacuumingRule.ToListing()));
    }

    [Fact]
    public void VacuumingRulesCanBeDeleted()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        DeleteVacuumingRule(process, TestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains($"Vacuuming rule '{TestVacuumingRule.ToListingIdentifier()}' successfully deleted"));
    }

    [Fact]
    public void VacuumingRulesCanReceivePurposes()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddStorageRule(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        AddPurposesToVacuumingRule(process, TestVacuumingRule, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"Vacuuming rule '{TestVacuumingRule.ToListingIdentifier()}' successfully updated")
            && s.Contains(NewTestPurpose.ToListingIdentifier())
            && s.Contains(VeryNewTestPurpose.ToListingIdentifier()));
    }

    [Fact]
    public void VacuumingRulesCanHavePurposesRemoved()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddStorageRule(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        AddPurposesToVacuumingRule(process, TestVacuumingRule, new[] { NewTestPurpose, VeryNewTestPurpose });
        RemovePurposesFromVacuumingRule(process, TestVacuumingRule, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"Vacuuming rule '{TestVacuumingRule.ToListingIdentifier()}' successfully updated")
            && !s.Contains(NewTestPurpose.ToListingIdentifier())
            && !s.Contains(VeryNewTestPurpose.ToListingIdentifier()));
    }

    [Fact]
    public void VacuumingRulesCanBeExecuted()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection);

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        UpdateStorageRuleWithPersonalDataColumn(process, TestStorageRule, TestPersonalDataColumn);
        AddVacuumingRule(process, TestVacuumingRule);
        
        ReportStatus(process);

        var statusOutput = process.GetLastOutputNoWhitespaceOrPrompt();
        statusOutput.Where(s => !s.Contains("Individual")).Should().BeEmpty();
        
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });

        // Operation made such that the ExecuteVacuumingRule has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespaceOrPrompt();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<string, VacuumingRule>(TestVacuumingRule.Key!,
                SystemOperation.Operation.Executed, null));
    }
    
    [Fact]
    public void ExecutingVacuumingRulesHandlesNullReferencesGracefullyStorageRuleMissingPersonalDataColumn()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection);

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        // UpdateStorageRuleWithPersonalDataColumn(process, TestStorageRule, TestPersonalDataColumn);
        AddVacuumingRule(process, TestVacuumingRule);
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });
        
        var output = process.GetLastOutputNoWhitespaceOrPrompt();
        output.Should()
            .Contain(
                FeedbackEmitterMessage.MissingMessage<string, StorageRule, PersonalDataColumn>(TestStorageRule.Key));

        // Operation made such that the ExecuteVacuumingRule has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        error.Should().BeEmpty();
    }
    
    [Fact]
    public void ExecutingVacuumingRulesHandlesNullReferencesGracefullyPurposeMissingStorageRule()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection);

        AddStorageRule(process, TestStorageRule);
        // AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        UpdateStorageRuleWithPersonalDataColumn(process, TestStorageRule, TestPersonalDataColumn);
        AddVacuumingRule(process, TestVacuumingRule);
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });
        
        var output = process.GetLastOutputNoWhitespaceOrPrompt();
        output.Should()
            .Contain(
                FeedbackEmitterMessage.MissingMessage<string, Purpose, StorageRule>(TestPurpose.Key));

        // Operation made such that the ExecuteVacuumingRule has enough time to report potential errors.
        AddOrigin(process, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();

        error.Should().BeEmpty();
    }

    [Fact]
    public void ExecutingVacuumingRulesAffectsData()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        TestStorageRule.VacuumingCondition = "Id = 2";

        SetupTestData(dbConnection);
        
        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        UpdateStorageRuleWithPersonalDataColumn(process, TestStorageRule, TestPersonalDataColumn);
        AddVacuumingRule(process, TestVacuumingRule);
        
        // Status should be empty, except regarding individuals missing origins
        ReportStatus(process);
        
        var statusOutput = process.GetLastOutputNoWhitespaceOrPrompt();
        statusOutput.Where(s => !s.Contains("Individual")).Should().BeEmpty();
        
        // Console should show that the vacuuming rule has been executed
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });
        
        var executeOutput = process.GetLastOutputNoWhitespaceOrPrompt().ToList();
        executeOutput.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.ResultMessage<string, VacuumingRule>(TestVacuumingRule.Key!,
                SystemOperation.Operation.Executed, null, null));
        executeOutput.Should().NotContain(s => s.Contains("had no effect"));
        
        // The log should contain entries regarding the vacuuming rules that have been executed
        ListLogs(process, new LogConstraints(logTypes: new [] { LogType.Vacuuming }));
        
        var logOutput = process.GetLastOutputNoWhitespaceOrPrompt().ToList();
        logOutput.Should().Contain(FeedbackEmitterMessage.ResultMessage<string, VacuumingRule>(TestVacuumingRule.Key!,
            SystemOperation.Operation.Executed, null, null));
        logOutput.Should().Contain(s => 
            s.Contains("WHERE") && 
            s.Contains(TestStorageRule.VacuumingCondition) && 
            s.Contains("affected") &&
            s.Contains(TestPersonalDataColumn.ToListingIdentifier()));

        // There should be no errors
        var error = process.GetAllErrorsNoWhitespace();
        error.Should().BeEmpty();

        var result = dbConnection.Query<(string key, string column)>(
                $"SELECT Id, {TestPersonalDataColumn.Key.ColumnName} " +
                $"FROM {TestPersonalDataColumn.Key.TableName}")
            .ToList();

        // The vacuuming condition should only affect one of the three rows, changing its value to the default value
        result.First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual1.ToListingIdentifier(), TestIndividual1.ToListingIdentifier()));
        result.Skip(1).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual2.ToListingIdentifier(), TestPersonalDataColumn.DefaultValue));
        result.Skip(2).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual3.ToListingIdentifier(), TestIndividual3.ToListingIdentifier()));
    }
}