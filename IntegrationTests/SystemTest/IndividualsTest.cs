using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class IndividualsTest : TestResources
{
    [Fact]
    public void IndividualsSourceCanBeSet()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        SetIndividualsSource(process, IndividualsSource);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains($"{IndividualsSource.ToListing()} successfully set"));
    }

    [Fact]
    public void IndividualsSourceCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        SetIndividualsSource(process, IndividualsSource);
        ShowIndividualsSource(process);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s == $"$: {IndividualsSource}");
    }

    [Fact]
    public void IndividualsCanBeListed()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        SetIndividualsSource(process, IndividualsSource);
        ListIndividuals(process);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(1.ToString()));
        output.Should().ContainSingle(s => s.Contains(2.ToString()));
        output.Should().ContainSingle(s => s.Contains(3.ToString()));
    }
}