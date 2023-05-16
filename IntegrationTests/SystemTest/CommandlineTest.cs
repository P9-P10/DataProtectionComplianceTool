using FluentAssertions;
using GraphManipulation.Commands.Helpers;
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
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        string error = process.GetError();
        result.Should().Contain(@$"Using config found at {process.ConfigPath}");
        result.Should().Contain("$: Description:  This is a description of the root command");
        result.Should().Contain("Usage:  ! [command] [options]");
        result.Should().Contain("Options:  ?, h, help  Show help and usage information");
        result.Should().Contain("Commands:  ");
        ContainsCommands(result);
    }

    private static void ContainsCommands(string result)
    {
        result.Should().Contain(CommandNamer.IndividualsName);
        result.Should().Contain(CommandNamer.IndividualsAlias);
        result.Should().Contain(CommandNamer.PersonalDataColumnsName);
        result.Should().Contain(CommandNamer.PersonalDataColumnsAlias);
        result.Should().Contain(CommandNamer.PurposesName);
        result.Should().Contain(CommandNamer.PurposesAlias);
        result.Should().Contain(CommandNamer.OriginsAlias);
        result.Should().Contain(CommandNamer.OriginsName);
        result.Should().Contain(CommandNamer.VacuumingRulesAlias);
        result.Should().Contain(CommandNamer.VacuumingRulesName);
        result.Should().Contain(CommandNamer.DeleteConditionsAlias);
        result.Should().Contain(CommandNamer.DeleteConditionsName);
        result.Should().Contain(CommandNamer.ProcessingsAlias);
        result.Should().Contain(CommandNamer.ProcessingsName);
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