namespace GraphManipulation.Logging;

public class MutableLog : IMutableLog
{
    public MutableLog(LogType logType, string subject, LogMessageFormat logMessageFormat, string message)
    {
        LogType = logType;
        Subject = subject;
        LogMessageFormat = logMessageFormat;
        Message = message;
    }

    public LogType LogType { get; set; }
    public string Subject { get; set; }
    public LogMessageFormat LogMessageFormat { get; set; }
    public string Message { get; set; }
}