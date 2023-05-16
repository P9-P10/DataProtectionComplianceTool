using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PurposeTest : TestResources
{
    [Fact]
    public void PurposeCanBeAdded()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => 
            s.Contains($"Successfully created {TestPurpose.Key} purpose"));
        output.Should().ContainSingle(s =>
            s.Contains($"Successfully updated {TestPurpose.Key} purpose with {TestPurpose.ToListing()}"));
    }

    [Fact]
    public void PurposeCanBeShown()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
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

        AddStorageRule(process, TestStorageRule);
        AddStorageRule(process, TestNewTestStorageRule);
        AddPurpose(process, TestPurpose);
        UpdatePurpose(process, TestPurpose, NewTestPurpose);
        ShowPurpose(process, NewTestPurpose);
        
        var error = process.GetAllErrors();
        var output = process.GetLastOutput();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(NewTestPurposeWithOldRule.ToListing()));
    }
    
    [Fact]
    public void PurposeCanBeListed()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, TestStorageRule);
        AddStorageRule(process, TestNewTestStorageRule);
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

        AddStorageRule(process, TestStorageRule);
        AddPurpose(process, TestPurpose);
        DeletePurpose(process, TestPurpose);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => 
            s.Contains($"Successfully deleted {TestPurpose.Key} purpose"));
    }
}