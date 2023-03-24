using System.Globalization;
using System.Text.RegularExpressions;
using GraphManipulation.Extensions;
using GraphManipulation.Helpers;

namespace GraphManipulation.Logging.Logs;

public enum LogType
{
    SchemaChange,
    Vacuuming,
    Metadata
}

public enum LogMessageFormat
{
    Json,
    Plaintext,
    Turtle
}

public class Log : ILog
{
    public int LogNumber { get; }
    public DateTime CreationTime { get; }
    public LogType LogType { get; }
    public LogMessageFormat LogMessageFormat { get; }
    public string Message { get; }

    public Log(int logNumber, DateTime creationTime, LogType logType, LogMessageFormat logMessageFormat, string message)
    {
        LogNumber = logNumber;
        CreationTime = creationTime;
        LogType = logType;
        LogMessageFormat = logMessageFormat;
        Message = message;
    }

    public Log(string logString)
    {
        var log = logString.StringToLog();

        LogNumber = log.LogNumber;
        CreationTime = log.CreationTime;
        LogType = log.LogType;
        LogMessageFormat = log.LogMessageFormat;
        Message = log.Message;
    }

    
    public static string LogDelimiter() => " <<>> ";

    private static string ValidLogStringPattern() =>
        $"^\\d+{LogDelimiter()}\\d+\\/\\d+\\/\\d+ \\d+\\.\\d+\\.\\d+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}.*$";

    public static bool IsValidLogString(string logString)
    {
        return Regex.IsMatch(logString, ValidLogStringPattern());
    }

    public string LogToString()
    {
        return LogNumber + LogDelimiter() +
               GetCreationTimeStamp() + LogDelimiter() +
               LogType + LogDelimiter() +
               LogMessageFormat + LogDelimiter() +
               Message;
    }

    public string GetCreationTimeStamp()
    {
        return CreationTime.ToShortDateString() + " " + CreationTime.ToLongTimeString();
    }

    protected bool Equals(Log other)
    {
        return CreationTime.Equals(other.CreationTime) && LogType == other.LogType &&
               LogMessageFormat == other.LogMessageFormat && Message == other.Message;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LogNumber, CreationTime, (int)LogType, (int)LogMessageFormat, Message);
    }
}

public class LogException : Exception
{
    public LogException(string message) : base(message)
    {
    }
}