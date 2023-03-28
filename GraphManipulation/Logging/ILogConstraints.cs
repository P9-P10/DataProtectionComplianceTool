using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogConstraints
{
    IOrderedEnumerable<ILog> ApplyConstraintsToLogs(IEnumerable<ILog> logs);
}