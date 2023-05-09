using FluentAssertions;
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
        result.Should().Be(@$"Using config found at {process.ConfigPath}" +
                           "$: Description:  This is a description of the root command" +
                           "Usage:  ! [command] [options]" +
                           "Options:  ?, h, help  Show help and usage information" +
                           "Commands:  " +
                           "ids, individuals  " +
                           "pd, personal-data  " +
                           "ps, purposes  " +
                           "origins, os  " +
                           "vacuuming-rules, vrs  " +
                           "dcs, delete-conditions  " +
                           "processings, prs  " +
                           "lgs, logs  " +
                           "cfg, configuration");
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