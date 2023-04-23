namespace GraphManipulation.Commands.BaseCommands;

public abstract class DeleteCommand : AliasedCommand
{
    public DeleteCommand(string? description = null) : base("delete", "d", description)
    {
    }
}