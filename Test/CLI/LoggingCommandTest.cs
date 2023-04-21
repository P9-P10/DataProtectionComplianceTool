using System.CommandLine;
using System.CommandLine.IO;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class LoggingCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<ILogger> loggerMock, out IConsole console)
    {
        console = new TestConsole();
        loggerMock = new Mock<ILogger>();
        
        return LoggingCommandBuilder.Build(console, loggerMock.Object);
    }

    public class List
    {
        private const string CommandName = "list";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var loggerMock, out _)
                .Invoke($"{CommandName}");
        }
    }
}