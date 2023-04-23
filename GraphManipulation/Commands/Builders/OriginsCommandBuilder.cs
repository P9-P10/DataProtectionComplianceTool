using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class OriginsCommandBuilder
{
    public static Command Build(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.OriginsName)
            .WithAlias(CommandNamer.OriginsAlias)
            .WithSubCommands(Add());
    }

    private static Command Add()
    {
        return CommandBuilder.BuildAddCommand();
    }

    private static Command Update()
    {
        return CommandBuilder.BuildUpdateCommand();
    }

    private static Command Delete()
    {
        return CommandBuilder.BuildDeleteCommand();
    }
}