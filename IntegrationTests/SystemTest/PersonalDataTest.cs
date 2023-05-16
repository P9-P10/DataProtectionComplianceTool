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

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);

        AddPersonalData(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully created {TestPersonalDataColumn.ToListingIdentifier()} personal data column"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestPersonalDataColumn.ToListingIdentifier()} personal data column with {TestPersonalDataColumn.ToListing()}"));
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
        output.Should().ContainSingle(s => s.Contains("$: " + TestPersonalDataColumn.ToListing()));
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
                $"Successfully deleted {TestPersonalDataColumn.ToListingIdentifier()} personal data column"));
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
                $"Successfully updated {TestPersonalDataColumn.ToListingIdentifier()} personal data column with {TestPersonalDataColumnWithMorePurposes.ToListing()}") &&
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
                $"Successfully updated {TestPersonalDataColumn.ToListingIdentifier()} personal data column with {TestPersonalDataColumn.ToListing()}") &&
            !s.Contains(NewTestPurpose.ToListingIdentifier()) && !s.Contains(VeryNewTestPurpose.ToListingIdentifier()));
    }

    [Fact]
    public void PersonalDataCanSetOriginForAnIndividual()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection);


        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddOrigin(process, TestOrigin);
        SetOriginOfPersonalData(process, TestPersonalDataColumn, TestIndividual1, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains($"Successfully completed set operation using " +
                                                      $"{TestPersonalDataColumn.ToListingIdentifier()}, " +
                                                      $"{TestIndividual1.ToListing()}, " +
                                                      $"{TestOrigin.ToListingIdentifier()}"));
    }

    [Fact]
    public void PersonalDataCanShowOriginForAnIndividual()
    {
        using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
        process.Start();
        process.AwaitReady();

        SetupTestData(dbConnection);

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddOrigin(process, TestOrigin);
        SetOriginOfPersonalData(process, TestPersonalDataColumn, TestIndividual1, TestOrigin);
        ShowOriginOfPersonalData(process, TestPersonalDataColumn, TestIndividual1);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestOrigin.ToListing()));
    }
}