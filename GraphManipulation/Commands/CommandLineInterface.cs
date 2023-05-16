using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Factories;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Logging;
using GraphManipulation.Utility;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands;

public class CommandLineInterface
{
    private ICommandHandlerFactory _commandHandlerFactory;
    private IManagerFactory _managerFactory;
    private Command _command;
    private Parser _parser;

    private readonly IVacuumerFactory _vacuumerFactory;
    private readonly ILoggerFactory _loggerFactory;

    public CommandLineInterface(IManagerFactory managerFactory, ILoggerFactory loggerFactory, IVacuumerFactory vacuumerFactory)
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
                new IndividualsCommandBuilder(_commandHandlerFactory, _managerFactory),
                new PersonalDataOriginCommandBuilder(_commandHandlerFactory, _managerFactory),
                new PersonalDataColumnCommandBuilder(_commandHandlerFactory, _managerFactory),
                new PurposesCommandBuilder(_commandHandlerFactory, _managerFactory),
                new OriginsCommandBuilder(_commandHandlerFactory),
                new VacuumingRulesCommandBuilder(_commandHandlerFactory, _managerFactory, _vacuumerFactory),
                new StorageRuleCommandBuilder(_commandHandlerFactory, _managerFactory),
                new ProcessingsCommandBuilder(_commandHandlerFactory, _managerFactory))
            .WithSubCommands(
                LoggingCommandBuilder.Build(_loggerFactory));
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
            .WithHandler(() => statusCommands.ToList().ForEach(statusCommand => statusCommand.Invoke(CommandNamer.Status)));

        _command = _command.WithSubCommands(allStatusCommand);
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