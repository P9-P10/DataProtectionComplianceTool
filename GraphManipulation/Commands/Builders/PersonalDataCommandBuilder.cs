using System.CommandLine;

namespace GraphManipulation.Commands.Builders;

public static class PersonalDataCommandBuilder
{
    public static Command Build()
    {
        return CommandBuilder.CreateCommand("personal-data")
            .WithAlias("pd")
            .WithSubCommand(AddPersonalData());
    }

    private static Command AddPersonalData()
    {
        return CommandBuilder.BuildAddCommand();
    }
}