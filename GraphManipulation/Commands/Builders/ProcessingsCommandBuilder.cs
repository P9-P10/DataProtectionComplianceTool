using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class ProcessingsCommandBuilder
{
    public static Command Build(IConsole console, IProcessingsManager processingsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.ProcessingsName)
            .WithAlias(CommandNamer.ProcessingsAlias)
            .WithSubCommands(AddProcessing());
    }

    private static Command AddProcessing()
    {
        return CommandBuilder.BuildAddCommand();
    }
}