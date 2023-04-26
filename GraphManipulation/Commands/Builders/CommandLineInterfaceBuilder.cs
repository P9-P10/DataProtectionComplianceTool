using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;

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
        return CommandBuilder.CreateCommand("c")
            .WithDescription("This is a description of the root command")
            .WithSubCommands(
                IndividualsCommandBuilder.Build(console, individualsManager),
                PersonalDataCommandBuilder.Build(console, personalDataManager, purposesManager,
                    originsManager, individualsManager),
                PurposesCommandBuilder.Build(console, purposesManager, deleteConditionsManager),
                OriginsCommandBuilder.Build(console, originsManager),
                VacuumingRulesCommandBuilder.Build(console, vacuumingRulesManager, purposesManager),
                DeleteConditionsCommandBuilder.Build(console, deleteConditionsManager),
                ProcessingsCommandBuilder.Build(console, processingsManager, personalDataManager, purposesManager),
                LoggingCommandBuilder.Build(console, logger),
                ConfigurationCommandBuilder.Build(console, configManager)
            );
    }
}