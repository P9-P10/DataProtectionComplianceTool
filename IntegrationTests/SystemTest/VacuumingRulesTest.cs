using Dapper;
using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class VacuumingRulesTest : TestResources
{
    [Fact]
    public void VacuumingRulesCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains($"Successfully created {TestVacuumingRule.ToListingIdentifier()} vacuuming rule"));
            output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestVacuumingRule.ToListingIdentifier()} vacuuming rule with {TestVacuumingRule.ToListing()}"));
    }

    [Fact]
    public void VacuumingRulesCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        ShowVacuumingRule(process, TestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(TestVacuumingRule.ToListing()));
    }

    [Fact]
    public void VacuumingRulesCanBeUpdated()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        UpdateVacuumingRule(process, TestVacuumingRule, UpdatedTestVacuumingRule);
        ShowVacuumingRule(process, UpdatedTestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(UpdatedTestVacuumingRule.ToListing()));
    }

    [Fact]
    public void VacuumingRulesCanBeListed()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        AddVacuumingRule(process, UpdatedTestVacuumingRule);
        ListVacuumingRule(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s => s.Contains(TestVacuumingRule.ToListing()));
        output.Should().ContainSingle(s => s.Contains(UpdatedTestVacuumingRule.ToListing()));
    }

    [Fact]
    public void VacuumingRulesCanBeDeleted()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        DeleteVacuumingRule(process, TestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully deleted {TestVacuumingRule.ToListingIdentifier()} vacuuming rule"));
    }

    [Fact]
    public void VacuumingRulesCanReceivePurposes()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestStorageRule);
        AddDeleteCondition(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        AddPurposesToVacuumingRule(process, TestVacuumingRule, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestVacuumingRule.ToListingIdentifier()} vacuuming rule with {NewTestPurpose.Key}"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestVacuumingRule.ToListingIdentifier()} vacuuming rule with {VeryNewTestPurpose.Key}"));
    }

    [Fact]
    public void VacuumingRulesCanHavePurposesRemoved()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestStorageRule);
        AddDeleteCondition(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddVacuumingRule(process, TestVacuumingRule);
        AddPurposesToVacuumingRule(process, TestVacuumingRule, new[] { NewTestPurpose, VeryNewTestPurpose });
        RemovePurposesFromVacuumingRule(process, TestVacuumingRule, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"{NewTestPurpose.Key} successfully removed from {TestVacuumingRule.ToListingIdentifier()}"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"{VeryNewTestPurpose.Key} successfully removed from {TestVacuumingRule.ToListingIdentifier()}"));
    }

    [Fact]
    public void VacuumingRulesCanBeExecuted()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection);

        AddDeleteCondition(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddVacuumingRule(process, TestVacuumingRule);
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });

        // Do something to get errors processed
        process.GiveInput("");

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully executed {TestVacuumingRule.ToListingIdentifier()} vacuuming rule"));
    }

    [Fact]
    public void ExecutingVacuumingRulesAffectsData()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        TestStorageRule.VacuumingCondition = "Id = 2";
        
        SetupTestData(dbConnection);

        AddDeleteCondition(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddVacuumingRule(process, TestVacuumingRule);
        ExecuteVacuumingRule(process, new[] { TestVacuumingRule });

        var result = dbConnection.Query<(string id, string column)>(
            $"SELECT Id, {TestPersonalDataColumn.Key.ColumnName} " +
            $"FROM {TestPersonalDataColumn.Key.TableName}")
            .ToList();

        result.First().Should().Be(new ValueTuple<string, string>(TestIndividual1.Id.ToString(), TestIndividual1.Key.ToString()));
        result.Skip(1).First().Should().Be(new ValueTuple<string, string>(TestIndividual2.Id.ToString(), TestIndividual2.Key.ToString()));
        result.Skip(2).First().Should().Be(new ValueTuple<string, string>(TestIndividual3.Id.ToString(), TestIndividual3.Key.ToString()));
    }
}