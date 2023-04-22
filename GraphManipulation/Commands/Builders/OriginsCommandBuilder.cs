using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class OriginsCommandBuilder
{
    public static Command Build(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.OriginsName)
            .WithAlias(CommandNamer.OriginsAlias)
            .WithSubCommands(AddOrigin());
    }

    private static Command AddOrigin()
    {
        return CommandBuilder.BuildAddCommand();
    }
}