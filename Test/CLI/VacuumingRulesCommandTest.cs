using System.CommandLine;
using System.CommandLine.IO;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class VacuumingRulesCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IVacuumingRulesManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IVacuumingRulesManager>();
        
        return VacuumingRulesCommandBuilder.Build(console, managerMock.Object);
    }

    public class Add
    {
        private const string CommandName = "add";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName}");
        }
    }
}