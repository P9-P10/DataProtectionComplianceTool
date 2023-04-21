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
            .WithSubCommand(ListIndividuals(console, individualsManager))
            .WithSubCommand(ShowIndividual(console, individualsManager));
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

    private static Command ListIndividuals(IConsole console, IIndividualsManager individualsManager)
    {

        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists all individuals currently in the system")
            .WithHandler(_ =>
            {
                individualsManager
                    .GetAll()
                    .ToList()
                    .ForEach(s => console.WriteLine(s.ToListing()));
            });
    }

    private static Command ShowIndividual(IConsole console, IIndividualsManager individualsManager)
    {
        var idOption = OptionBuilder
            .CreateIdOption("The id of the individual to be shown")
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows information pertaining to the individual with the given id")
            .WithOption(idOption);
        
        command.SetHandler(id =>
        {
            var individual = individualsManager.Get(id);
            
            console.WriteLine(individual != null ? individual.ToListing() : "Could not find individual with that id");
        }, idOption);


        return command;
    }
}