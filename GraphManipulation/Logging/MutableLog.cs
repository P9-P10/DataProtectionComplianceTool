using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class MutableLog : IMutableLog
{
    public LogType LogType { get; set; }
    public LogMessageFormat LogMessageFormat { get; set; }
    public string Message { get; set; }

    public MutableLog(LogType logType, LogMessageFormat logMessageFormat, string message)
    {
        LogType = logType;
        LogMessageFormat = logMessageFormat;
        Message = message;
    }
}