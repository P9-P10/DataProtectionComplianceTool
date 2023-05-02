using FluentAssertions;
using GraphManipulation.Commands.Helpers;

namespace Test.SystemTest;

[Collection("SystemTestSequential")]
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
        process.GiveInput($"{CommandNamer.OriginsAlias} {CommandNamer.List}");
        List<string> result = process.GetLastOutput();

        result.Should().Contain($"{name}, {description}, [  ]");
    }

    [Fact]
    public void TestUpdateOrigin_Returns_Correct_Message()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        process.GiveInput($"{CommandNamer.OriginsAlias} {CommandNamer.AddAlias} {OptionNamer.Name} {name} {OptionNamer.Description} \"{description}\"");
        
        
        process.GiveInput($"{CommandNamer.OriginsAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name} {name} {OptionNamer.NewName} NewName");
        List<string> result = process.GetLastOutput();

        result.First().Should().Contain("NewName");
        result.First().Should().Contain(name);
        result.First().Should().Contain("Successfully");
    }
}