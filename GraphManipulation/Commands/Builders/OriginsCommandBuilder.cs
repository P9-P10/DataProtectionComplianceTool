using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class OriginsCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("origins")
            .WithAlias("os")
            .WithSubCommand(AddOrigin());
    }

    private static Command AddOrigin()
    {
        return CommandBuilder.BuildAddCommand();
    }
}