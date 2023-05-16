using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Factories;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands;

public class HandlerFactory : IHandlerFactory {
    private readonly IManagerFactory _managerFactory;

    public HandlerFactory(IManagerFactory managerFactory)
    {
        _managerFactory = managerFactory;
    }
    
    public ICommandHandler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport) where TV : Entity<TK>, new() where TK : notnull
    {
        IManager<TK, TV> manager = _managerFactory.CreateManager<TK, TV>();
        CommandHandler<TK, TV> commandHandler = new CommandHandler<TK, TV>(manager, emitter, statusReport);
        return commandHandler;
    }
}

public class CommandLineInterface
{
    private IHandlerFactory _handlerFactory;
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
        _handlerFactory = new HandlerFactory(managerFactory);
        
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
                new IndividualsCommandBuilder(_handlerFactory, _managerFactory),
                new PersonalDataOriginCommandBuilder(_handlerFactory, _managerFactory),
                new PersonalDataColumnCommandBuilder(_handlerFactory, _managerFactory),
                new PurposesCommandBuilder(_handlerFactory, _managerFactory),
                new OriginsCommandBuilder(_handlerFactory),
                new VacuumingRulesCommandBuilder(_handlerFactory, _managerFactory, _vacuumerFactory),
                new StorageRuleCommandBuilder(_handlerFactory, _managerFactory),
                new ProcessingsCommandBuilder(_handlerFactory, _managerFactory))
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