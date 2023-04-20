using GraphManipulation.Commands.BaseCommands;
using GraphManipulation.Helpers;

namespace GraphManipulation.Commands.CompositeCommands;

public class ConfigurationCommand : AliasedCommand
{
    public ConfigurationCommand(IConfigManager configManager, string? description = null) 
        : base("config", "cfg", description)
    {
    }
}