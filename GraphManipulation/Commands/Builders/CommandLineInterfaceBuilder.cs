using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class CommandLineInterfaceBuilder
{
    public static RootCommand Build(
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
        return RootCommandBuilder.CreateRootCommand("This is a description of the command")
            .WithCommand(IndividualsCommandBuilder.Build(console, individualsManager))
            .WithCommand(PersonalDataCommandBuilder.Build(console, personalDataManager))
            .WithCommand(PurposesCommandBuilder.Build(console, purposesManager))
            .WithCommand(OriginsCommandBuilder.Build(console, originsManager))
            .WithCommand(VacuumingRulesCommandBuilder.Build(console, vacuumingRulesManager))
            .WithCommand(DeleteConditionsCommandBuilder.Build(console, deleteConditionsManager))
            .WithCommand(ProcessingsCommandBuilder.Build(console, processingsManager))
            .WithCommand(LoggingCommandBuilder.Build(console, logger))
            .WithCommand(ConfigurationCommandBuilder.Build(console, configManager));
    }
}



