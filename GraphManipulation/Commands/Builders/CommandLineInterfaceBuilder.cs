using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class CommandLineInterfaceBuilder
{
    public static RootCommand Build()
    {
        return RootCommandBuilder.CreateRootCommand("This is a description of the command")
            .WithCommand(IndividualsCommandBuilder.Build())
            .WithCommand(PersonalDataCommandBuilder.Build())
            .WithCommand(PurposesCommandBuilder.Build())
            .WithCommand(OriginsCommandBuilder.Build())
            .WithCommand(VacuumingRulesCommandBuilder.Build())
            .WithCommand(DeleteConditionsCommandBuilder.Build())
            .WithCommand(ProcessingsCommandBuilder.Build())
            .WithCommand(LoggingCommandBuilder.Build())
            .WithCommand(ConfigurationCommandBuilder.Build());
    }
}



