using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Factories;

public class CommandHandlerFactory : ICommandHandlerFactory {
    private readonly IManagerFactory _managerFactory;

    public CommandHandlerFactory(IManagerFactory managerFactory)
    {
        _managerFactory = managerFactory;
    }
    
    public ICommandHandler<TK, TV> CreateCommandHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport) where TV : Entity<TK>, new() where TK : notnull
    {
        IManager<TK, TV> manager = _managerFactory.CreateManager<TK, TV>();
        CommandHandler<TK, TV> commandHandler = new CommandHandler<TK, TV>(manager, emitter, statusReport);
        return commandHandler;
    }
}