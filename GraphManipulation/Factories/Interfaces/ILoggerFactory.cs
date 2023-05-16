using GraphManipulation.Logging;

namespace GraphManipulation.Factories.Interfaces;

public interface ILoggerFactory
{
    public ILogger CreateLogger();
}