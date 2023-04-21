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
            .WithSubCommand(SetSource(individualsManager))
            .WithSubCommand(ListIndividuals(individualsManager))
            .WithSubCommand(ShowIndividual());
    }

    private static Command SetSource(IIndividualsManager individualsManager)
    {
        var tableOption = OptionBuilder
            .CreateOption<string>("--table")
            .WithAlias("-t")
            .WithDescription("The table in which the individuals can be found")
            .WithIsRequired(true);

        var columnOption = OptionBuilder
            .CreateOption<string>("--column")
            .WithAlias("-c")
            .WithDescription("The column in which the IDs of the individuals are stored")
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildSetCommand("source")
            .WithDescription("Sets the source of individuals for whom personal data can be managed")
            .WithOption(tableOption)
            .WithOption(columnOption);
        
        command.SetHandler((tableName, columnName) =>
        {
            individualsManager.SetIndividualsSource(new TableColumnPair(tableName, columnName));
        }, tableOption, columnOption);
        
        return command;
    }

    private static Command ListIndividuals(IIndividualsManager individualsManager)
    {

        var command = CommandBuilder
            .BuildListCommand();

        command.SetHandler(() =>
        {
            individualsManager
                .GetAll()
                .ToList()
                .ForEach(s => Console.WriteLine(s.ToListing()));
        });
        
        return command;
    }

    private static Command ShowIndividual()
    {
        return CommandBuilder.BuildShowCommand();
    }
}