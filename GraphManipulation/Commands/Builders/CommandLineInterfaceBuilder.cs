using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class CommandLineInterfaceBuilder
{
    public static RootCommand Build(
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
            .WithCommand(IndividualsCommandBuilder.Build(individualsManager))
            .WithCommand(PersonalDataCommandBuilder.Build(personalDataManager))
            .WithCommand(PurposesCommandBuilder.Build(purposesManager))
            .WithCommand(OriginsCommandBuilder.Build(originsManager))
            .WithCommand(VacuumingRulesCommandBuilder.Build(vacuumingRulesManager))
            .WithCommand(DeleteConditionsCommandBuilder.Build(deleteConditionsManager))
            .WithCommand(ProcessingsCommandBuilder.Build(processingsManager))
            .WithCommand(LoggingCommandBuilder.Build(logger))
            .WithCommand(ConfigurationCommandBuilder.Build(configManager));
    }
}



