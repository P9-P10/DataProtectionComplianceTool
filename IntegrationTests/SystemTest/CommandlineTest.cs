using FluentAssertions;
using GraphManipulation.Commands;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class CommandlineTest
{
    [Fact]
    public void TestHelpCommand()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        process.GiveInput("help");
        
        var result = string.Join("", process.GetAllOutputNoWhitespace());
        var error = process.GetAllErrorsNoWhitespace();
        
        error.Should().BeEmpty();
        
        result.Should().Contain($"Using config found at {process.ConfigPath}");
        result.Should().Contain("Description:");
        result.Should().Contain("Usage:! [command] [options]");
        result.Should().Contain("Options:?, h, help  Show help and usage information");
        result.Should().Contain("Commands:");
        ContainsCommands(result);
    }

    private static void ContainsCommands(string result)
    {
        result.Should().Contain(CommandNamer.IndividualName);
        result.Should().Contain(CommandNamer.IndividualAlias);
        result.Should().Contain(CommandNamer.PersonalDataColumnName);
        result.Should().Contain(CommandNamer.PersonalDataColumnAlias);
        result.Should().Contain(CommandNamer.PurposeName);
        result.Should().Contain(CommandNamer.PurposeAlias);
        result.Should().Contain(CommandNamer.OriginAlias);
        result.Should().Contain(CommandNamer.OriginName);
        result.Should().Contain(CommandNamer.VacuumingPolicyAlias);
        result.Should().Contain(CommandNamer.VacuumingPolicyName);
        result.Should().Contain(CommandNamer.StoragePolicyName);
        result.Should().Contain(CommandNamer.StoragePolicyName);
        result.Should().Contain(CommandNamer.ProcessingAlias);
        result.Should().Contain(CommandNamer.ProcessingName);
        result.Should().Contain(CommandNamer.LoggingAlias);
        result.Should().Contain(CommandNamer.LoggingName);
    }

    [Fact]
    public void TestWithError()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        process.GiveInput("please break");
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        string error = process.GetError();
        result.Should().NotBeEmpty();
        error.Should().NotBeEmpty();
        error.Should().Be($"Required command was not provided.{Environment.NewLine}Unrecognized command or argument 'please'.{Environment.NewLine}Unrecognized command or argument 'break'.{Environment.NewLine}");
    }
}