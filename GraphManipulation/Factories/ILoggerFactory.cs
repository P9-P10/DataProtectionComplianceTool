using GraphManipulation.Logging;

namespace GraphManipulation.Factories;

public interface ILoggerFactory
{
    public ILogger CreateLogger();
}