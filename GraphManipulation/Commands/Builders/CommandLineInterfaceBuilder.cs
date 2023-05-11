using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;
using IOriginsManager = GraphManipulation.Managers.Interfaces.IOriginsManager;

namespace GraphManipulation.Commands.Builders;

public static class CommandLineInterfaceBuilder
{
    public static Command Build(
        IConsole console,
        IIndividualsManager individualsManager,
        IPersonalDataManager personalDataManager,
        IPurposesManager purposesManager,
        IOriginsManager originsManager,
        IVacuumingRulesManager vacuumingRulesManager,
        IDeleteConditionsManager deleteConditionsManager,
        IProcessingsManager processingsManager,
        ILogger logger,
        IConfigManager configManager)
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.RootCommandName)
            .WithAlias(CommandNamer.RootCommandAlias)
            .WithDescription("This is a description of the root command")
            .WithSubCommands(
                IndividualsCommandBuilder.Build(console, individualsManager),
                // PersonalDataCommandBuilder.Build(console, personalDataManager, purposesManager,
                //     originsManager, individualsManager),
                PurposesCommandBuilder.Build(console, purposesManager, deleteConditionsManager),
                new OriginsCommandBuilder(console, originsManager).Build(),
                VacuumingRulesCommandBuilder.Build(console, vacuumingRulesManager, purposesManager),
                DeleteConditionsCommandBuilder.Build(console, deleteConditionsManager),
                ProcessingsCommandBuilder.Build(console, processingsManager, personalDataManager, purposesManager),
                LoggingCommandBuilder.Build(console, logger),
                ConfigurationCommandBuilder.Build(console, configManager)
            );
    }
}