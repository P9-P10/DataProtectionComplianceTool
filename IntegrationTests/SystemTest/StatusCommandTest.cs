using FluentAssertions;
using GraphManipulation.Commands;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class StatusCommandTest : TestResources
{
    private static IEnumerable<string> FormatOutputForStatusTest(IEnumerable<string> list)
    {
        return list
            .Select(s => s.Replace(CommandLineInterface.Prompt, "").Trim())
            .Where(s => s.Contains("missing"));
    }

    private static string UniqueStringKey => "UniqueStringKey";
    
    [Fact]
    public void NothingToReportEmptyOutput()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        ReportStatus(process);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetAllOutputNoWhitespace());

        error.Should().BeEmpty();
        output.Should().BeEmpty();
    }

    [Fact]
    public void CreatingStorageRule()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        process.GiveInput($"{CommandNamer.StorageRulesName} {CommandNamer.Create} {OptionNamer.Name} {UniqueStringKey}");
        ReportStatus(process);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetAllOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();

        output.First().Should().Be(FeedbackEmitterMessage
            .MissingMessage<string, StorageRule>(UniqueStringKey, "vacuuming condition"));
        output.Skip(1).First().Should().Be(FeedbackEmitterMessage
            .MissingMessage<string, StorageRule, PersonalDataColumn>(UniqueStringKey));
    }

    [Fact]
    public void CreatingPurpose()
    {
        true.Should().Be(false);
    }
}