using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;

namespace GraphManipulation.Commands.Builders;

public static class IndividualsCommandBuilder
{
    public static Command Build(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.IndividualsName)
            .WithAlias(CommandNamer.IndividualsAlias)
            .WithSubCommands(
                SetSource(console, individualsManager),
                ShowSource(console, individualsManager),
                ListIndividuals(console, individualsManager),
                ShowIndividual(console, individualsManager)
            );
    }

    private static Command SetSource(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder
            .BuildSetCommand("source")
            .WithDescription("Sets the source of individuals for whom personal data can be managed")
            .WithOption(out var pairOption,
                OptionBuilder
                    .CreateTableColumnPairOption()
                    .WithDescription("The table and column in which the individuals can be found, should be written as: Table Column"))
            .WithHandler(context =>
                Handlers.SetHandler(context, console, individualsManager.SetIndividualsSource, pairOption));
    }

    private static Command ShowSource(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder
            .BuildShowCommand("source")
            .WithDescription("Shows the current source of individuals")
            .WithHandler(() => console.WriteLine(individualsManager.GetIndividualsSource().ToListing()));
    }

    private static Command ListIndividuals(IConsole console, IIndividualsManager individualsManager)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists all individuals currently in the system")
            .WithHandler(() => Handlers.ListHandler(console, individualsManager,CommandHeader.IndividualsHeader));
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
            .WithHandler(context => Handlers.ShowHandler(context, console, individualsManager, idOption));
    }
}