using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILoggerConstraints
{
    IOrderedEnumerable<ILog> Apply(IEnumerable<ILog> logs);
}