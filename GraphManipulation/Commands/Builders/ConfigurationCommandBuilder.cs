using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class ConfigurationCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("configuration")
            .WithAlias("cfg")
            .WithSubCommand(UpdateConfig());
    }

    private static Command UpdateConfig()
    {
        return CommandBuilder.BuildUpdateCommand();
    }
}