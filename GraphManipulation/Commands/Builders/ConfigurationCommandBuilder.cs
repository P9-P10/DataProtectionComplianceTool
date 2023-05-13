using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;

namespace GraphManipulation.Commands.Builders;

public static class ConfigurationCommandBuilder
{
    public static Command Build(IConfigManager configManager)
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.ConfigurationName)
            .WithAlias(CommandNamer.ConfigurationAlias);
        // .WithSubCommands(UpdateConfig());
    }

    private static Command UpdateConfig()
    {
        return CommandBuilder.BuildUpdateCommand();
    }
}