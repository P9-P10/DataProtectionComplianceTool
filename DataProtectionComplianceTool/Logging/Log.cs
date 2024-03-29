using System.Globalization;
using System.Text.RegularExpressions;

namespace GraphManipulation.Logging;

public class Log : ILog
{
    public Log(int logNumber, DateTime creationTime, LogType logType, string subject, LogMessageFormat logMessageFormat,
        string message)
    {
        LogNumber = logNumber;
        CreationTime = creationTime;
        LogType = logType;
        Subject = subject;
        LogMessageFormat = logMessageFormat;
        Message = message;
    }

    public Log(string logString)
    {
        LogNumber = LogStringParser.ParseLogNumber(logString);
        CreationTime = LogStringParser.ParseCreationTime(logString);
        LogType = LogStringParser.ParseLogType(logString);
        Subject = LogStringParser.ParseSubject(logString);
        LogMessageFormat = LogStringParser.ParseLogMessageFormat(logString);
        Message = LogStringParser.ParseLogMessage(logString);
    }

    public Log(int logNumber, DateTime creationTime, IMutableLog mutableLog) :
        this(logNumber, creationTime, mutableLog.LogType, mutableLog.Subject, mutableLog.LogMessageFormat,
            mutableLog.Message)
    {
    }

    public int LogNumber { get; }
    public DateTime CreationTime { get; }
    public LogType LogType { get; }
    public string Subject { get; }
    public LogMessageFormat LogMessageFormat { get; }
    public string Message { get; }

    public string GetCreationTimeStamp()
    {
        return CreationTime.ToString(CreationTimeStampFormat(), CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        return LogNumber + LogDelimiter() +
               GetCreationTimeStamp() + LogDelimiter() +
               LogType + LogDelimiter() +
               Subject + LogDelimiter() +
               LogMessageFormat + LogDelimiter() +
               Message;
    }

    public static string LogDelimiter()
    {
        // WARNING: MUST NOT BE SET TO ANY REGEX OPERATORS,
        // SUCH AS | or . or ^ or $ etc., OTHERWISE EVERYTHING BREAKS.
        // USING ESCAPED REGEX OPERATORS IS NOT AN OPTION. 
        return "   ";
    }

    /// <summary>
    ///     Describes a pattern for a valid log string of the form: "LogNumber {LogDelimiter} CreationTimeStamp {LogDelimiter}
    ///     LogType {LogDelimiter} Subject {LogDelimiter} LogMessageFormat {LogDelimiter} Message"
    ///     A valid string could be: "36   12/05/2027 11.56.45   Vacuuming   TestSubject   Plaintext   This is a vacuuming
    ///     message"
    /// </summary>
    /// <returns></returns>
    // 
    private static string ValidLogStringPattern()
    {
        return
            $"^\\d+{LogDelimiter()}\\d+-\\d+-\\d+ \\d+:\\d+:\\d+{LogDelimiter()}\\w+{LogDelimiter()}.+{LogDelimiter()}\\w+{LogDelimiter()}.*$";
    }

    public static bool IsValidLogString(string logString)
    {
        return Regex.IsMatch(logString, ValidLogStringPattern());
    }

    public static string CreationTimeStampFormat()
    {
        return "dd-MM-yyyy HH:mm:ss";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LogNumber, CreationTime, (int)LogType, (int)LogMessageFormat, Message);
    }
}