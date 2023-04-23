using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Logging;

namespace GraphManipulation.Commands.Builders;

public static class LoggingCommandBuilder
{
    public static Command Build(IConsole console, ILogger logger)
    {
        return CommandBuilder.CreateCommand(CommandNamer.LoggingName)
            .WithAlias(CommandNamer.LoggingAlias)
            .WithSubCommands(ListLog());
    }

    private static Command ListLog()
    {
        return CommandBuilder.BuildListCommand();
    }
}