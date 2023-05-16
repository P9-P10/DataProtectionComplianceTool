using GraphManipulation.Commands.Helpers;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Factories;

public interface IHandlerFactory
{
    public IHandler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport)
        where TV : Entity<TK>, new() where TK : notnull;
}