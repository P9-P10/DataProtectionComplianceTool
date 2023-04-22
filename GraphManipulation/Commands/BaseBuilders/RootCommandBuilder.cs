using System.CommandLine;

namespace GraphManipulation.Commands.BaseBuilders;

public static class RootCommandBuilder
{
    public static RootCommand CreateRootCommand(string description)
    {
        return new RootCommand(description);
    }

    public static RootCommand WithCommands(this RootCommand rootCommand, params Command[] commands)
    {
        foreach (var subCommand in commands)
        {
            rootCommand.AddCommand(subCommand);
        }

        return rootCommand;
    }
}