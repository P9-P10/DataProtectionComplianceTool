using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands.Builders;

public static class CommandLineInterfaceBuilder
{
    public static Command Build(
        IManager<int, Individual> individualsManager,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager,
        IManager<string, Purpose> purposesManager,
        IManager<string, Origin> originsManager,
        IManager<string, VacuumingRule> vacuumingRulesManager,
        IManager<string, DeleteCondition> deleteConditionsManager,
        IManager<string, Processing> processingsManager,
        IVacuumer vacuumer,
        ILogger logger,
        IConfigManager configManager)
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.RootCommandName)
            .WithAlias(CommandNamer.RootCommandAlias)
            .WithDescription("This is a description of the root command")
            .WithSubCommands(
                new IndividualsCommandBuilder(individualsManager, personalDataColumnManager, originsManager),
                new PersonalDataColumnCommandBuilder(personalDataColumnManager, purposesManager),
                new PurposesCommandBuilder(purposesManager, deleteConditionsManager),
                new OriginsCommandBuilder(originsManager),
                new VacuumingRulesCommandBuilder(vacuumingRulesManager, purposesManager, vacuumer),
                new DeleteConditionsCommandBuilder(deleteConditionsManager, personalDataColumnManager),
                new ProcessingsCommandBuilder(processingsManager, purposesManager, personalDataColumnManager))
            .WithSubCommands(
                LoggingCommandBuilder.Build(logger),
                ConfigurationCommandBuilder.Build(configManager));
    }
}