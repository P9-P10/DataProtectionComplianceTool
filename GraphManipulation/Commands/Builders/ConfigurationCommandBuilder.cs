using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Helpers;

namespace GraphManipulation.Commands.Builders;

public static class ConfigurationCommandBuilder
{
    public static Command Build(IConsole console, IConfigManager configManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.ConfigurationName)
            .WithAlias(CommandNamer.ConfigurationAlias)
            .WithSubCommand(UpdateConfig());
    }

    private static Command UpdateConfig()
    {
        return CommandBuilder.BuildUpdateCommand();
    }
}