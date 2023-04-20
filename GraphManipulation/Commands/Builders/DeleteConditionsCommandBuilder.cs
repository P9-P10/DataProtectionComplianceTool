using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class DeleteConditionsCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("delete-conditions")
            .WithAlias("dcs")
            .WithSubCommand(AddDeleteCondition());
    }

    private static Command AddDeleteCondition()
    {
        return CommandBuilder.BuildAddCommand();
    }
}