using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;
using Lucene.Net.Queries.Mlt;

namespace GraphManipulation.Commands.Builders;

public static class VacuumingRulesCommandBuilder
{
    public static Command Build(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.VacuumingRulesName)
            .WithAlias(CommandNamer.VacuumingRulesAlias)
            .WithSubCommand(AddVacuumingRule());
    }

    private static Command AddVacuumingRule()
    {
        return CommandBuilder.BuildAddCommand();
    }
}