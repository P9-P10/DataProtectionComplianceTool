using System.CommandLine;

namespace GraphManipulation.Commands.BaseCommands;

public abstract class AliasedCommand : Command
{
    public AliasedCommand(string name, string alias, string? description = null) : base(name, description)
    {
        AddAlias(alias);
    }
}