using System.CommandLine.Invocation;

namespace GraphManipulation.Commands.Builders;

public class CommandHandler : ICommandHandler
{
    private readonly List<Action<InvocationContext>> _handles;

    public CommandHandler()
    {
        _handles = new List<Action<InvocationContext>>();
    }

    public int Invoke(InvocationContext context)
    {
        if (_handles.Count == 0)
        {
            throw new CommandHandlerException("No handles to invoke");
        }

        foreach (var handle in _handles)
            handle(context);

        return context.ExitCode;
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public void AddHandle(Action<InvocationContext> handle)
    {
        _handles.Add(handle);
    }
}

public static class CommandHandlerExtensions
{
    public static CommandHandler WithHandle(this CommandHandler commandHandler, Action<InvocationContext> handle)
    {
        commandHandler.AddHandle(handle);
        return commandHandler;
    }
}

public class CommandHandlerException : Exception
{
    public CommandHandlerException(string message) : base(message)
    {
    }
}