using System.CommandLine;
using GraphManipulation.Commands.CompositeCommands;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.MetadataManagement;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands;

// https://github.com/dotnet/command-line-api
public sealed class CommandLineInterface : RootCommand
{
    public CommandLineInterface(IMetadataManager metadataManager, IVacuumer vacuumer, ILogger logger,
        IConfigManager configManager)
    {
        Description = "This is a description of the command";
        AddCommand(new MetadataEntriesCommand(metadataManager));
        AddCommand(new VacuumingRulesCommand(vacuumer));
        AddCommand(new LinkingCommand(metadataManager, vacuumer));
        AddCommand(new LoggingCommand(logger));
        AddCommand(new ConfigurationCommand(configManager));
    }
}