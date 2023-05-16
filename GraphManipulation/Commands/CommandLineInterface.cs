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
    
    public Handler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport) where TV : Entity<TK>, new() where TK : notnull
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
    
    public CommandLineInterface(IHandlerFactory handlerFactory, IManagerFactory managerFactory)
    {
        _handlerFactory = handlerFactory;
        _managerFactory = managerFactory;
        // Create subcommands
        _command = Build();
        AddStatusCommands();
        CreateCommandParser();
    }

    public void Invoke(string command)
    {
        _parser.Invoke(command);
    }
    
    private Command Build()
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.RootCommandName)
            .WithAlias(CommandNamer.RootCommandAlias)
            .WithDescription("This is a description of the root command")
            .WithSubCommands(
                new IndividualsCommandBuilder(_handlerFactory, _managerFactory),
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

    private void AddStatusCommands()
    {
        // What the heck is going on here?
        _command = _command.WithSubCommands(
            CommandBuilder
                .BuildCreateCommand()
                .WithHandler(() =>
                {
                    _command.Subcommands
                        .ToList()
                        .ForEach(subCommand =>
                        {
                            subCommand.Subcommands
                                .Where(subSubCommand => subSubCommand.Name == CommandNamer.Status)
                                .ToList()
                                .ForEach(sub =>
                                {
                                    sub.Invoke(CommandNamer.Status);
                                });
                        });
                })
        );
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