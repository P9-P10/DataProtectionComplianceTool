using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public static class CommandLineInterfaceBuilder
{
    public static Command Build(
        IConsole console,
        IManager<int, Individual> individualsManager,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager,
        IManager<string, Purpose> purposesManager,
        IManager<string, Origin> originsManager,
        IManager<string, VacuumingRule> vacuumingRulesManager,
        IManager<string, DeleteCondition> deleteConditionsManager,
        IManager<string, Processing> processingsManager,
        ILogger logger,
        IConfigManager configManager)
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.RootCommandName)
            .WithAlias(CommandNamer.RootCommandAlias)
            .WithDescription("This is a description of the root command")
            .WithSubCommands(
                // IndividualsCommandBuilder.Build(console, individualsManager),
                new PersonalDataColumnCommandBuilder(console, personalDataColumnManager, purposesManager, originsManager, individualsManager).Build(),
                new PurposesCommandBuilder(console, purposesManager, deleteConditionsManager).Build(),
                new OriginsCommandBuilder(console, originsManager).Build(),
                // VacuumingRulesCommandBuilder.Build(console, vacuumingRulesManager, purposesManager),
                new DeleteConditionsCommandBuilder(console, deleteConditionsManager, personalDataColumnManager).Build(),
                new ProcessingsCommandBuilder(console, processingsManager, purposesManager, personalDataColumnManager).Build(),
                LoggingCommandBuilder.Build(console, logger),
                ConfigurationCommandBuilder.Build(console, configManager)
            );
    }
}