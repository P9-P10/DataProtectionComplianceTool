using System.CommandLine;

namespace GraphManipulation.Commands.BaseBuilders;

public static class RootCommandBuilder
{
    public static RootCommand CreateRootCommand(string description)
    {
        return new RootCommand(description);
    }
    
    public static RootCommand WithCommand(this RootCommand command, Command subCommand)
    {
        command.AddCommand(subCommand);
        return command;
    }
}