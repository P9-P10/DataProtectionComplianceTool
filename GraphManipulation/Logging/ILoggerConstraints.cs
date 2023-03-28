using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILoggerConstraints
{
    IOrderedEnumerable<ILog> ApplyConstraintsToLogs(IEnumerable<ILog> logs);
}