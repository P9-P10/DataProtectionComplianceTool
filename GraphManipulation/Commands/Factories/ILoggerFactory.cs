using GraphManipulation.Logging;

namespace GraphManipulation.Commands.Factories;

public interface ILoggerFactory
{
    public ILogger CreateLogger();
}