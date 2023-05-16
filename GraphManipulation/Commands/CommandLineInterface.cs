using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Commands.Factories;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands;

public class HandlerFactory : IHandlerFactory {
    private readonly IManagerFactory _managerFactory;

    public HandlerFactory(IManagerFactory managerFactory)
    {
        _managerFactory = managerFactory;
    }
    
    public IHandler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport) where TV : Entity<TK>, new() where TK : notnull
    {
        IManager<TK, TV> manager = _managerFactory.CreateManager<TK, TV>();
        Handler<TK, TV> handler = new Handler<TK, TV>(manager, emitter, statusReport);
        return handler;
    }
}

public class CommandLineInterface
{
    private IHandlerFactory _handlerFactory;
    private IManagerFactory _managerFactory;
    private Command _command;
    private Parser _parser;

    private readonly IVacuumer _vacuumer;
    private readonly ILogger _logger;
    private readonly IConfigManager _configManager;
    
    public CommandLineInterface(IManagerFactory managerFactory, ILogger logger, IVacuumer vacuumer, IConfigManager configManager)
    {
        _managerFactory = managerFactory;
        _handlerFactory = new HandlerFactory(managerFactory);
        _logger = logger;
        _vacuumer = vacuumer;
        _configManager = configManager;
        
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
                new VacuumingRulesCommandBuilder(_handlerFactory, _managerFactory, _vacuumer),
                new StorageRuleCommandBuilder(_handlerFactory, _managerFactory),
                new ProcessingsCommandBuilder(_handlerFactory, _managerFactory))
            .WithSubCommands(
                LoggingCommandBuilder.Build(_logger),
                ConfigurationCommandBuilder.Build(_configManager));
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