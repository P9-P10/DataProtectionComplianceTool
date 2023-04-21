using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PersonalDataCommandBuilder
{
    public static Command Build(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PersonalDataName)
            .WithAlias(CommandNamer.PersonalDataAlias)
            .WithSubCommand(AddPersonalData());
    }

    private static Command AddPersonalData()
    {
        return CommandBuilder.BuildAddCommand();
    }
}