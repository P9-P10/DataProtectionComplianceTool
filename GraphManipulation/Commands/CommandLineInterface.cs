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
    
    public IVacuumer Vacuumer { get; set; }
    public ILogger Logger { get; set; }
    public IConfigManager ConfigManager { get; set; }
    
    public CommandLineInterface(IManagerFactory managerFactory)
    {
        _managerFactory = managerFactory;
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
                new VacuumingRulesCommandBuilder(_handlerFactory, _managerFactory, Vacuumer),
                new DeleteConditionsCommandBuilder(_handlerFactory, _managerFactory),
                new ProcessingsCommandBuilder(_handlerFactory, _managerFactory))
            .WithSubCommands(
                LoggingCommandBuilder.Build(Logger),
                ConfigurationCommandBuilder.Build(ConfigManager));
    }

    private void AddAllStatusCommand()
    {
        var subCommands = _command.Subcommands;
        var statusCommands =
            subCommands.Select(subCommand => subCommand.Subcommands.First(c => c.Name == CommandNamer.Status));
        
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