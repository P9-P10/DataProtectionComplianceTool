using FluentAssertions;
using GraphManipulation.Commands.Helpers;
using Xunit;

namespace Test.SystemTest;

public class TestOrigins
{
    [Fact]
    public void TestAddOrigin_Returns_Correct_Message()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        process.GiveInput($"{CommandNamer.OriginsAlias} {CommandNamer.AddAlias} {OptionNamer.Name} {name} {OptionNamer.Description} \"{description}\"");
        string result = process.GetOutput();

        result.Should().Contain(description);
        result.Should().Contain(name);
        result.Should().Contain("Successfully");
    }
    
    [Fact]
    public void TestAddOrigin_Origin_Stored_Correctly()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        process.GiveInput($"{CommandNamer.OriginsAlias} {CommandNamer.AddAlias} {OptionNamer.Name} {name} {OptionNamer.Description} \"{description}\"");
        string result = process.GetOutput();

        result.Should().Contain(description);
        result.Should().Contain(name);
        result.Should().Contain("Successfully");
    }
}