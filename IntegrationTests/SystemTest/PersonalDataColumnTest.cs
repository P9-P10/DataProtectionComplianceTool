using FluentAssertions;
using GraphManipulation.Commands;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PersonalDataColumnTest : TestResources
{
    [Fact]
    public void PersonalDataCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);

        AddPersonalDataColumn(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();
        
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<TableColumnPair, PersonalDataColumn>(TestPersonalDataColumn.Key!,
                SystemOperation.Operation.Created, null));
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestPersonalDataColumn.Key!,
                SystemOperation.Operation.Updated, TestPersonalDataColumn));
    }

    [Fact]
    public void PersonalDataCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        ShowPersonalDataColumn(process, TestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains($"{CommandLineInterface.Prompt} " + TestPersonalDataColumn.ToListing()));
    }

    [Fact]
    public void PersonalDataCanBeUpdated()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        UpdatePersonalDataColumn(process, TestPersonalDataColumn, UpdatedTestPersonalDataColumn);
        ShowPersonalDataColumn(process, UpdatedTestPersonalDataColumn);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespace();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(UpdatedTestPersonalDataColumn.ToListing()));
    }

    [Fact]
    public void PersonalDataCanBeListed()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddStoragePolicy(process, NewTestStoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        AddPersonalDataColumn(process, NewTestPersonalDataColumn);
        ListPersonalDataColumn(process);

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

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        DeletePersonalDataColumn(process, TestPersonalDataColumn);

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

        AddStoragePolicy(process, StoragePolicy);
        AddStoragePolicy(process, NewTestStoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        AddPurposesToPersonalDataColumn(process, TestPersonalDataColumn, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt();

        TestPersonalDataColumn.Purposes =
            TestPersonalDataColumn.Purposes.Append(NewTestPurpose).Append(VeryNewTestPurpose);

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestPersonalDataColumn.Key!,
                SystemOperation.Operation.Updated, TestPersonalDataColumn));
    }

    [Fact]
    public void PersonalDataCanHavePurposesRemoved()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddStoragePolicy(process, NewTestStoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        AddPurpose(process, VeryNewTestPurpose);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        AddPurposesToPersonalDataColumn(process, TestPersonalDataColumn, new[] { NewTestPurpose, VeryNewTestPurpose });
        RemovePurposesFromPersonalDataColumn(process, TestPersonalDataColumn, new[] { NewTestPurpose, VeryNewTestPurpose });

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestPersonalDataColumn.Key!,
                SystemOperation.Operation.Updated, TestPersonalDataColumn));
    }
    
    [Fact]
    public void PersonalDataCanReceiveLegalBases()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddLegalBasis(process, TestLegalBasis);
        AddLegalBasis(process, UpdatedTestLegalBasis);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        AddLegalBasesToPersonalDataColumn(process, TestPersonalDataColumn, new[] { UpdatedTestLegalBasis });
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt();

        TestPersonalDataColumn.LegalBases = TestPersonalDataColumn.LegalBases.Append(UpdatedTestLegalBasis);
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestPersonalDataColumn.Key!,
                SystemOperation.Operation.Updated, TestPersonalDataColumn));
    }

    [Fact]
    public void PersonalDataCanHaveLegalBasesRemoved()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        AddLegalBasis(process, TestLegalBasis);
        AddLegalBasis(process, UpdatedTestLegalBasis);
        AddPersonalDataColumn(process, TestPersonalDataColumn);
        AddLegalBasesToPersonalDataColumn(process, TestPersonalDataColumn, new[] { UpdatedTestLegalBasis });
        RemoveLegalBasesFromPersonalDataColumn(process, TestPersonalDataColumn, new []{UpdatedTestLegalBasis});
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt().ToList();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestPersonalDataColumn.Key!,
                SystemOperation.Operation.Updated, TestPersonalDataColumn));
    }
}