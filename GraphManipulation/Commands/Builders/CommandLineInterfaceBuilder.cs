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
                new IndividualsCommandBuilder(individualsManager, personalDataColumnManager, originsManager).Build(),
                new PersonalDataColumnCommandBuilder(personalDataColumnManager, purposesManager).Build(),
                new PurposesCommandBuilder(purposesManager, deleteConditionsManager).Build(),
                new OriginsCommandBuilder(originsManager).Build(),
                new VacuumingRulesCommandBuilder(vacuumingRulesManager, purposesManager, vacuumer).Build(),
                new DeleteConditionsCommandBuilder(deleteConditionsManager, personalDataColumnManager).Build(),
                new ProcessingsCommandBuilder(processingsManager, purposesManager, personalDataColumnManager).Build(),
                LoggingCommandBuilder.Build(logger),
                ConfigurationCommandBuilder.Build(configManager)
            );
    }
}