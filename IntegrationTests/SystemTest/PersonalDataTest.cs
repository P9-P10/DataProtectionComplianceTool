using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PersonalDataTest : TestResources
{
    [Fact]
    public void PersonalDataCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);

        AddPersonalData(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully added {TestPersonalDataColumn.TableColumnPair.ToListing()} personal data column") &&
            s.Contains($"{TestPersonalDataColumn.GetDescription()}") &&
            s.Contains($"{TestPersonalDataColumn.GetJoinCondition()}"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestPersonalDataColumn.TableColumnPair.ToListing()} personal data column with {TestPurpose.GetName()}"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestPersonalDataColumn.TableColumnPair.ToListing()} personal data column with {TestPersonalDataColumn.GetDefaultValue()}"));
    }

    [Fact]
    public void PersonalDataCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        ShowPersonalData(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPersonalDataColumn.ToListing()));
    }

    [Fact]
    public void PersonalDataCanBeUpdated()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        UpdatePersonalData(process, TestPersonalDataColumn, UpdatedTestPersonalDataColumn);
        ShowPersonalData(process, UpdatedTestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(UpdatedTestPersonalDataColumn.ToListing()));
    }

    [Fact]
    public void PersonalDataCanBeListed()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddDeleteCondition(process, NewTestDeleteCondition);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddPersonalData(process, NewTestPersonalDataColumn);
        ListPersonalData(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPersonalDataColumn.ToListing()));
        output.Should().ContainSingle(s => s.Contains(NewTestPersonalDataColumn.ToListing()));
    }

    [Fact]
    public void PersonalDataCanBeDeleted()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        DeletePersonalData(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully deleted {TestPersonalDataColumn.GetTableColumnPair().ToListing()} personal data column"));
    }

    [Fact]
    public void PersonalDataCanReceivePurposes()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddDeleteCondition(process, NewTestDeleteCondition);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, NewNewTestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        PersonalDataAddPurposes(process, TestPersonalDataColumn, new[] { NewTestPurpose, NewNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestPersonalDataColumn.TableColumnPair.ToListing()} personal data column with {NewTestPurpose.GetName()}"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestPersonalDataColumn.TableColumnPair.ToListing()} personal data column with {NewNewTestPurpose.GetName()}"));
    }

    [Fact]
    public void PersonalDataCanHavePurposesRemoved()
    {
    }

    [Fact]
    public void PersonalDataCanSetOriginForAnIndividual()
    {
    }

    [Fact]
    public void PersonalDataCanShowOriginForAnIndividual()
    {
    }
}