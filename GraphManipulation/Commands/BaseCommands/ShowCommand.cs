namespace GraphManipulation.Commands.BaseCommands;

public abstract class ShowCommand : AliasedCommand
{
    public ShowCommand(string? description = null) : base("show", "s", description)
    {
    }
}