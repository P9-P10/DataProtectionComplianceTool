using System.CommandLine;
using GraphManipulation.Commands.Helpers;
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
        return CommandBuilder
            .BuildSetCommand("source")
            .WithDescription("Sets the source of individuals for whom personal data can be managed")
            .WithOption(out var pairOption,
                OptionBuilder
                    .CreateTableColumnPairOption()
                    .WithDescription("The table and column in which the individuals can be found"))
            .WithHandler(new CommandHandler()
                .WithHandle(context =>
                    BaseBuilder.AddHandler(context, individualsManager.SetIndividualsSource, pairOption)));
    }

    private static Command ListIndividuals(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists all individuals currently in the system")
            .WithHandler(new CommandHandler()
                .WithHandle(_ => BaseBuilder.ListHandler(console, individualsManager)));
    }

    private static Command ShowIndividual(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows information pertaining to the individual with the given id")
            .WithOption(out var idOption, 
                OptionBuilder
                    .CreateIdOption()
                    .WithDescription("The id of the individual to be shown")
                    .WithIsRequired(true))
            .WithHandler(new CommandHandler()
                .WithHandle(context => BaseBuilder.ShowHandler(context, console, individualsManager, idOption)));
    }
}