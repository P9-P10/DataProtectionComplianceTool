using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogger
{
    public ILog CreateLog(LogType logType, LogMessageFormat logMessageFormat, string message);
    public void Append(ILog log);
    public void CreateAndAppendLog(LogType logType, LogMessageFormat logMessageFormat, string message);
    public IOrderedEnumerable<ILog> Read(ILoggerConstraints constraints);
}