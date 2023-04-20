using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class LoggingCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("logs")
            .WithAlias("lgs")
            .WithSubCommand(ShowLog());
    }

    private static Command ShowLog()
    {
        return CommandBuilder.BuildShowCommand();
    }
}