using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Factories;

public interface IHandlerFactory
{
    public ICommandHandler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport)
        where TV : Entity<TK>, new() where TK : notnull;
}