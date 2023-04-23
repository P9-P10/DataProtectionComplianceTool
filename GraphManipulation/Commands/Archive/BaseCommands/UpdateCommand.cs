namespace GraphManipulation.Commands.BaseCommands;

public abstract class UpdateCommand : AliasedCommand
{
    public UpdateCommand(string? description = null) : base("update", "u", description)
    {
    }
}