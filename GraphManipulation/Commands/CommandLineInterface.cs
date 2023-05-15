using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Decorators;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands;

public interface IManagerFactory
{
    public IManager<TK, TV> CreateManager<TK, TV>() where TV : Entity<TK>, new() where TK : notnull;
}

public class ManagerFactory : IManagerFactory
{
    private GdprMetadataContext _dbContext;

    public ManagerFactory(GdprMetadataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IManager<TK, TV> CreateManager<TK, TV>() where TV : Entity<TK>, new() where TK : notnull
    {
        return new Manager<TK, TV>(new Mapper<TV>(_dbContext));
    }
}

public class LoggingManagerFactory : IManagerFactory
{
    private IManagerFactory _managerFactory;
    private readonly ILogger _logger;

    public LoggingManagerFactory(IManagerFactory managerFactory, ILogger logger)
    {
        _managerFactory = managerFactory;
        _logger = logger;
    }

    public IManager<TK, TV> CreateManager<TK, TV>() where TV : Entity<TK>, new() where TK : notnull
    {
        return new LoggingManager<TK, TV>(_managerFactory.CreateManager<TK, TV>(), _logger);
    }
}

public interface IVacuumerFactory
{
    public IVacuumer CreateVacuumer();
}

public interface ILoggerFactory
{
    public ILogger CreateLogger();
}

public interface IConfigManagerFactory
{
    public IConfigManager CreateConfigManager();
}

public interface IComponentFactory : IManagerFactory, IVacuumerFactory, ILoggerFactory, IConfigManagerFactory, IHandlerFactory
{
    
}
public class ComponentFactory : IComponentFactory
{
    private readonly IManagerFactory _managerFactory;
    private readonly IVacuumerFactory _vacuumerFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConfigManagerFactory _configManagerFactory;
    private readonly IHandlerFactory _handlerFactory;

    public ComponentFactory(IManagerFactory managerFactory, IVacuumerFactory vacuumerFactory,
        ILoggerFactory loggerFactory, IConfigManagerFactory configManagerFactory)
    {
        _managerFactory = managerFactory;
        _vacuumerFactory = vacuumerFactory;
        _loggerFactory = loggerFactory;
        _configManagerFactory = configManagerFactory;
        _handlerFactory = new HandlerFactory(managerFactory);
    }
    public IManager<TK, TV> CreateManager<TK, TV>() where TK : notnull where TV : Entity<TK>, new()
    {
        return _managerFactory.CreateManager<TK, TV>();
    }

    public IVacuumer CreateVacuumer()
    {
        return _vacuumerFactory.CreateVacuumer();
    }

    public ILogger CreateLogger()
    {
        return _loggerFactory.CreateLogger();
    }

    public IConfigManager CreateConfigManager()
    {
        return _configManagerFactory.CreateConfigManager();
    }

    public Handler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport) where TK : notnull where TV : Entity<TK>, new()
    {
        return _handlerFactory.CreateHandler(emitter, statusReport);
    }
}

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
    private IComponentFactory _componentFactory;
    private Command _command;
    private Parser _parser;
    
    public CommandLineInterface(IComponentFactory componentFactory)
    {
        _componentFactory = componentFactory;
        // Create subcommands
        _command = Build(componentFactory);
        AddStatusCommands();
        CreateCommandParser();
    }

    public void Invoke(string command)
    {
        _parser.Invoke(command);
    }
    
    private Command Build(IComponentFactory factory)
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.RootCommandName)
            .WithAlias(CommandNamer.RootCommandAlias)
            .WithDescription("This is a description of the root command")
            .WithSubCommands(
                new IndividualsCommandBuilder(factory, factory),
                new PersonalDataColumnCommandBuilder(factory, factory),
                new PurposesCommandBuilder(factory, factory),
                new OriginsCommandBuilder(factory),
                new VacuumingRulesCommandBuilder(factory, factory, factory.CreateVacuumer()),
                new DeleteConditionsCommandBuilder(factory, factory),
                new ProcessingsCommandBuilder(factory, factory))
            .WithSubCommands(
                LoggingCommandBuilder.Build(factory.CreateLogger()),
                ConfigurationCommandBuilder.Build(factory.CreateConfigManager()));
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