using Dapper;
using FluentAssertions;
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
        var output = process.GetAllOutputNoWhiteSpaceOrPrompt().ToList();

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
        AddVacuumingRule(process, TestVacuumingRule);
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });

        // Operation made such that the ExecuteVacuumingRule has enough time to report potential errors.
        AddStorageRule(process, TestStorageRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains($"Vacuuming rule '{TestVacuumingRule.ToListingIdentifier()}' successfully executed"));
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
        AddVacuumingRule(process, TestVacuumingRule);
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });

        var result = dbConnection.Query<(string id, string column)>(
                $"SELECT Id, {TestPersonalDataColumn.Key.ColumnName} " +
                $"FROM {TestPersonalDataColumn.Key.TableName}")
            .ToList();

        result.First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual1.ToListingIdentifier(), TestIndividual1.ToListingIdentifier()));
        result.Skip(1).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual2.ToListingIdentifier(), TestPersonalDataColumn.DefaultValue));
        result.Skip(2).First().Should()
            .Be(new ValueTuple<string, string>(TestIndividual3.ToListingIdentifier(), TestIndividual3.ToListingIdentifier()));
    }
}