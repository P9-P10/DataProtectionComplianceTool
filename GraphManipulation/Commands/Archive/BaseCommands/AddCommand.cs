namespace GraphManipulation.Commands.BaseCommands;

public abstract class AddCommand : AliasedCommand
{
    public AddCommand(string? description = null) : base("add", "a", description)
    {
    }
}