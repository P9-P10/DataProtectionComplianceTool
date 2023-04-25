using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class VacuumingRulesCommandBuilder
{
    public static Command Build(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.VacuumingRulesName)
            .WithAlias(CommandNamer.VacuumingRulesAlias)
            .WithSubCommands(AddVacuumingRule());
    }

    private static Command AddVacuumingRule()
    {
        return CommandBuilder.BuildAddCommand();
    }
}