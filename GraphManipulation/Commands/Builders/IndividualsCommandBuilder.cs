using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class IndividualsCommandBuilder
{
    public static Command Build(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.IndividualsName)
            .WithAlias(CommandNamer.IndividualsAlias)
            .WithSubCommands(
                SetSource(individualsManager),
                ListIndividuals(console, individualsManager),
                ShowIndividual(console, individualsManager)
            );
    }

    private static Command SetSource(IIndividualsManager individualsManager)
    {
        var tableOption = OptionBuilder
            .CreateTableOption()
            .WithDescription("The table in which the individuals can be found")
            .WithIsRequired(true);

        var columnOption = OptionBuilder
            .CreateColumnOption()
            .WithDescription("The column in which the IDs of the individuals are stored")
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildSetCommand("source")
            .WithDescription("Sets the source of individuals for whom personal data can be managed")
            .WithOptions(tableOption, columnOption);

        command.SetHandler(
            (tableName, columnName) =>
            {
                individualsManager.SetIndividualsSource(new TableColumnPair(tableName, columnName));
            }, tableOption, columnOption);

        return command;
    }

    private static Command ListIndividuals(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder
            .BuildListCommand(console, individualsManager)
            .WithDescription("Lists all individuals currently in the system");
    }

    private static Command ShowIndividual(IConsole console, IIndividualsManager individualsManager)
    {
        var idOption = OptionBuilder
            .CreateIdOption()
            .WithDescription("The id of the individual to be shown")
            .WithIsRequired(true);

        return CommandBuilder
            .BuildShowCommand(console, individualsManager, idOption, "individual")
            .WithDescription("Shows information pertaining to the individual with the given id")
            .WithOptions(idOption);
    }
}