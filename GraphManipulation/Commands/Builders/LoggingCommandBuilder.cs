using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Logging;

namespace GraphManipulation.Commands.Builders;

public static class LoggingCommandBuilder
{
    public static Command Build(ILogger logger)
    {
        return CommandBuilder.CreateCommand(CommandNamer.LoggingName)
            .WithAlias(CommandNamer.LoggingAlias)
            .WithSubCommand(ShowLog());
    }

    private static Command ShowLog()
    {
        return CommandBuilder.BuildShowCommand();
    }
}