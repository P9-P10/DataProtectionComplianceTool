using GraphManipulation.Commands.Helpers;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands;

public interface IHandlerFactory
{
    public IHandler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport)
        where TV : Entity<TK>, new() where TK : notnull;
}