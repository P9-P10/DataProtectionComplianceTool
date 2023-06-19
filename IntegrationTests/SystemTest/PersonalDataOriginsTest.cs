using FluentAssertions;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PersonalDataOriginsTest : TestResources
{
    [Fact]
    public void PersonalDataOriginCanSetOriginForAnIndividual()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateIndividual(process, TestIndividual1);
        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        AddOrigin(process, TestOrigin);
        CreatePersonalDataOrigin(process, 1, TestPersonalDataColumn, TestIndividual1, TestOrigin);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<int, PersonalDataOrigin>(1, SystemOperation.Operation.Created,
                null));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage.SuccessMessage(1,
            SystemOperation.Operation.Updated, new PersonalDataOrigin
            {
                Key = 1, Individual = TestIndividual1, Origin = TestOrigin, PersonalDataColumn = TestPersonalDataColumn
            }));
    }
}