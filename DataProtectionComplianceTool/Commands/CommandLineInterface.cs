using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Factories;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands;

public class CommandLineInterface
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly IVacuumerFactory _vacuumerFactory;
    private Command _command;
    private readonly ICommandHandlerFactory _commandHandlerFactory;
    private readonly IManagerFactory _managerFactory;
    private Parser _parser;

    public CommandLineInterface(IManagerFactory managerFactory, ILoggerFactory loggerFactory,
        IVacuumerFactory vacuumerFactory)
    {
        _managerFactory = managerFactory;
        _loggerFactory = loggerFactory;
        _vacuumerFactory = vacuumerFactory;
        _commandHandlerFactory = new CommandHandlerFactory(managerFactory);

        // Create subcommands
        CreateCommand();
        AddAllStatusCommand();
        CreateCommandParser();
    }

    public static char Prompt => '$';

    public void Invoke(string command)
    {
        _parser.Invoke(command);
    }

    private void CreateCommand()
    {
        _command = CommandBuilder.CreateNewCommand(CommandNamer.RootCommandName)
            .WithAlias(CommandNamer.RootCommandAlias)
            .WithDescription("This is a description of the root command")
            .WithSubCommands(
                new IndividualCommandBuilder(_commandHandlerFactory, _managerFactory),
                new PersonalDataOriginCommandBuilder(_commandHandlerFactory, _managerFactory),
                new PersonalDataColumnCommandBuilder(_commandHandlerFactory, _managerFactory),
                new PurposeCommandBuilder(_commandHandlerFactory, _managerFactory),
                new OriginCommandBuilder(_commandHandlerFactory),
                new VacuumingPolicyCommandBuilder(_commandHandlerFactory, _managerFactory, _vacuumerFactory),
                new StoragePolicyCommandBuilder(_commandHandlerFactory, _managerFactory),
                new ProcessingCommandBuilder(_commandHandlerFactory, _managerFactory))
            .WithSubCommands(
                LoggingCommandBuilder.Build(_loggerFactory),
                QuitCommand());
    }

    private void AddAllStatusCommand()
    {
        var subCommands = _command.Subcommands;
        var statusCommands = subCommands
            .Select(subCommand => subCommand.Subcommands.FirstOrDefault(c => c.Name == CommandNamer.Status))
            .Where(command => command is not null);

        var allStatusCommand = CommandBuilder
            .BuildStatusCommand()
            .WithDescription("Shows the status of all entities in the system")
            .WithHandler(() =>
                statusCommands.ToList().ForEach(statusCommand => statusCommand.Invoke(CommandNamer.Status)));

        _command = _command.WithSubCommands(allStatusCommand);
    }

    private static Command QuitCommand()
    {
        return CommandBuilder
            .CreateNewCommand(CommandNamer.Quit)
            .WithAlias(CommandNamer.QuitAlias)
            .WithDescription("Quits the program")
            .WithHandler(() => Environment.Exit(0));
    }

    private void CreateCommandParser()
    {
        _parser = new CommandLineBuilder(_command)
            .UseHelp("help", "h", "?")
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .Build();
    }
}