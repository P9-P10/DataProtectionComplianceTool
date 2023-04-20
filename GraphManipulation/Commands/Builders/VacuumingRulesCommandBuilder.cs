using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class VacuumingRulesCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("vacuuming-rules")
            .WithAlias("vrs")
            .WithSubCommand(AddVacuumingRule());
    }

    private static Command AddVacuumingRule()
    {
        return CommandBuilder.BuildAddCommand();
    }
}