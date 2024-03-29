using FluentAssertions;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PurposeTest : TestResources
{
    [Fact]
    public void PurposeCanBeAdded()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<string, Purpose>(TestPurpose.Key!,
                SystemOperation.Operation.Created, null));
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestPurpose.Key!,
                SystemOperation.Operation.Updated, TestPurpose));
    }

    [Fact]
    public void PurposeCanBeShown()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        ShowPurpose(process, TestPurpose);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPurpose.ToListing()));
    }

    [Fact]
    public void PurposeCanBeUpdated()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddStoragePolicy(process, NewTestStoragePolicy);
        AddPurpose(process, TestPurpose);
        UpdatePurpose(process, TestPurpose, NewTestPurpose);
        ShowPurpose(process, NewTestPurpose);
        
        var error = process.GetAllErrors();
        var output = process.GetLastOutput();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(NewTestPurposeWithOldPolicy.ToListing()));
    }
    
    [Fact]
    public void PurposeCanBeListed()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddStoragePolicy(process, NewTestStoragePolicy);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        ListPurpose(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutput().ToList();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPurpose.ToListing()));
        output.Should().ContainSingle(s => s.Contains(NewTestPurpose.ToListing()));
    }

    [Fact]
    public void PurposeCanBeDeleted()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, StoragePolicy);
        AddPurpose(process, TestPurpose);
        DeletePurpose(process, TestPurpose);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => 
            s.Contains($"Purpose '{TestPurpose.Key}' successfully deleted"));
        
    }
}