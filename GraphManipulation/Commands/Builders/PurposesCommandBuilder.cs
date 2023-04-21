using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PurposesCommandBuilder
{
    public static Command Build(IConsole console, IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PurposesName)
            .WithAlias(CommandNamer.PurposesAlias)
            .WithSubCommand(AddPurpose());
    }

    private static Command AddPurpose()
    {
        return CommandBuilder.BuildAddCommand();
    }
}