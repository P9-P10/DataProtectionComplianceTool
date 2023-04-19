using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class ProcessingsCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("processings")
            .WithAlias("prs")
            .WithSubCommand(AddProcessing());
    }

    private static Command AddProcessing()
    {
        return CommandBuilder.BuildAddCommand();
    }
}