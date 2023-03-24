using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogger
{
    public ILog CreateLog(LogType logType, LogMessageFormat logMessageFormat, string message);
    public void Append(ILog log);
    public IOrderedEnumerable<ILog> Read(LoggerReadOptions options);
}