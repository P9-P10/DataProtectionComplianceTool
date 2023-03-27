using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class PlaintextLog : ILog
{
    public PlaintextLog(int logNumber, DateTime creationTime, LogType logType, LogMessageFormat logMessageFormat,
        string message)
    {
        LogNumber = logNumber;
        CreationTime = creationTime;
        LogType = logType;
        LogMessageFormat = logMessageFormat;
        Message = message;
    }

    public PlaintextLog(string logString)
    {
        LogNumber = LogStringParser.ParseLogNumber(logString);
        CreationTime = LogStringParser.ParseCreationTime(logString);
        LogType = LogStringParser.ParseLogType(logString);
        LogMessageFormat = LogStringParser.ParseLogMessageFormat(logString);
        Message = LogStringParser.ParseLogMessage(logString);
    }

    public int LogNumber { get; }
    public DateTime CreationTime { get; }
    public LogType LogType { get; }
    public LogMessageFormat LogMessageFormat { get; }
    public string Message { get; }

    public override string ToString()
    {
        return LogNumber + BaseLogger.LogDelimiter() +
               GetCreationTimeStamp() + BaseLogger.LogDelimiter() +
               LogType + BaseLogger.LogDelimiter() +
               LogMessageFormat + BaseLogger.LogDelimiter() +
               Message;
    }

    public string GetCreationTimeStamp()
    {
        return CreationTime.ToShortDateString() + " " + CreationTime.ToLongTimeString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LogNumber, CreationTime, (int)LogType, (int)LogMessageFormat, Message);
    }
}