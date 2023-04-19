using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class PurposesCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("purposes")
            .WithAlias("ps")
            .WithSubCommand(AddPurpose());
    }

    private static Command AddPurpose()
    {
        return CommandBuilder.BuildAddCommand();
    }
}