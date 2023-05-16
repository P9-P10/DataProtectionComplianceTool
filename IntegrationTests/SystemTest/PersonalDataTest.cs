using FluentAssertions;
using GraphManipulation.Commands;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PersonalDataTest : TestResources
{
    [Fact]
    public void PersonalDataCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);

        AddPersonalData(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Personal data column '{TestPersonalDataColumn.ToListingIdentifier()}' successfully created"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Personal data column '{TestPersonalDataColumn.ToListingIdentifier()}' successfully updated with {TestPersonalDataColumn.ToListing()}"));
    }

    [Fact]
    public void PersonalDataCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        ShowPersonalData(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains($"{CommandLineInterface.Prompt} " + TestPersonalDataColumn.ToListing()));
    }

    [Fact]
    public void PersonalDataCanBeUpdated()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
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

        AddStorageRule(process, TestStorageRule);
        AddStorageRule(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddPersonalData(process, NewTestPersonalDataColumn);
        ListPersonalData(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPersonalDataColumn.ToListing()));
        output.Should().ContainSingle(s => s.Contains(NewTestPersonalDataColumn.ToListing()));
    }

    [Fact]
    public void PersonalDataCanBeDeleted()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        DeletePersonalData(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Personal data column '{TestPersonalDataColumn.ToListingIdentifier()}' successfully deleted"));
    }

    [Fact]
    public void PersonalDataCanReceivePurposes()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddStorageRule(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddPurposesToPersonalData(process, TestPersonalDataColumn, new[] {NewTestPurpose, VeryNewTestPurpose});

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Personal data column '{TestPersonalDataColumn.ToListingIdentifier()}' successfully updated with {TestPersonalDataColumnWithMorePurposes.ToListing()}") &&
            s.Contains(NewTestPurpose.ToListingIdentifier()) && s.Contains(VeryNewTestPurpose.ToListingIdentifier()));
        
        
    }

    [Fact]
    public void PersonalDataCanHavePurposesRemoved()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddStorageRule(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddPurposesToPersonalData(process, TestPersonalDataColumn, new[] {NewTestPurpose, VeryNewTestPurpose});
        RemovePurposesFromPersonalData(process, TestPersonalDataColumn, new[] {NewTestPurpose, VeryNewTestPurpose});

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Personal data column '{TestPersonalDataColumn.ToListingIdentifier()}' successfully updated with {TestPersonalDataColumn.ToListing()}") &&
            !s.Contains(NewTestPurpose.ToListingIdentifier()) && !s.Contains(VeryNewTestPurpose.ToListingIdentifier()));
    }


}