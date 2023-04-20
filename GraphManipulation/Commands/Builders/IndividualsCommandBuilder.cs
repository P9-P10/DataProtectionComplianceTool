using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class IndividualsCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("individuals")
            .WithAlias("ids")
            .WithSubCommand(SetSource());
    }

    private static Command SetSource()
    {
        return CommandBuilder.BuildSetCommand("source");
    }
}