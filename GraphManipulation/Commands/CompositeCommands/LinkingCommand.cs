using GraphManipulation.Commands.BaseCommands;
using GraphManipulation.MetadataManagement;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands.CompositeCommands;

public class LinkingCommand : AliasedCommand
{
    public LinkingCommand(IMetadataManager metadataManager, IVacuumer vacuumer, string? description = null) 
        : base("linking", "lk", description)
    {
    }
}