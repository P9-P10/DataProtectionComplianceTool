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

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => 
            s.Contains($"Successfully added {TestPurpose.GetName()} purpose") &&
            s.Contains($"{TestPurpose.GetLegallyRequired()}") && 
            s.Contains(TestPurpose.GetDescription())
            );
        output.Should().ContainSingle(s =>
            s.Contains($"Successfully updated {TestPurpose.GetName()} purpose with {TestDeleteCondition.GetName()}"));
    }

    [Fact]
    public void PurposeCanBeShown()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
        ShowPurpose(process, TestPurpose);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPurpose.ToListing()));
    }

    [Fact]
    public void PurposeCanBeUpdated()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddDeleteCondition(process, NewTestDeleteCondition);
        AddPurpose(process, TestPurpose);
        UpdatePurpose(process, TestPurpose, NewTestPurpose);
        ShowPurpose(process, NewTestPurpose);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(NewTestPurpose.ToListing()));
    }
    
    [Fact]
    public void PurposeCanBeListed()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddDeleteCondition(process, NewTestDeleteCondition);
        AddPurpose(process, TestPurpose);
        AddPurpose(process, NewTestPurpose);
        ListPurpose(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPurpose.ToListing()));
        output.Should().ContainSingle(s => s.Contains(NewTestPurpose.ToListing()));
    }

    [Fact]
    public void PurposeCanBeDeleted()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
        DeletePurpose(process, TestPurpose);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => 
            s.Contains($"Successfully deleted {TestPurpose.GetName()} purpose"));
    }
}