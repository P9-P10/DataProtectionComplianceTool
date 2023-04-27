using System.CommandLine;
using FluentAssertions;

namespace Test.CLI;

public class CommandTest
{
    protected static void VerifyCommand(Command cli, string command, bool happy = true)
    {
        if (happy)
        {
            cli.Parse(command).Errors.Should().BeEmpty();
        }
        else
        {
            cli.Parse(command).Errors.Should().NotBeEmpty();
        }
        
        cli.Invoke(command).Should().Be(happy ? 0 : 1);
    }
}