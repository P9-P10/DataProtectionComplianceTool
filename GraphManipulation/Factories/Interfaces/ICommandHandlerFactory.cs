using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Factories.Interfaces;

public interface ICommandHandlerFactory
{
    public ICommandHandler<TK, TV> CreateCommandHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport)
        where TV : Entity<TK>, new() where TK : notnull;
}