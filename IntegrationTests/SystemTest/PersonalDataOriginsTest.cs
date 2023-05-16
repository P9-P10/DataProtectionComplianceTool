using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PersonalDataOriginsTest : TestResources
{
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