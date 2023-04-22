using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class DeleteConditionsCommandBuilder
{
    public static Command Build(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.DeleteConditionName)
            .WithAlias(CommandNamer.DeleteConditionAlias)
            .WithSubCommands(AddDeleteCondition());
    }

    private static Command AddDeleteCondition()
    {
        return CommandBuilder.BuildAddCommand();
    }
}