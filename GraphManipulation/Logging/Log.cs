using System.Text.RegularExpressions;
using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public abstract class Log : ILog
{
    public int LogNumber { get; }
    public DateTime CreationTime { get; }
    public LogType LogType { get; }
    public LogMessageFormat LogMessageFormat { get; }
    public string Message { get; }
    
    protected Log(int logNumber, DateTime creationTime, LogType logType, LogMessageFormat logMessageFormat,
        string message)
    {
        LogNumber = logNumber;
        CreationTime = creationTime;
        LogType = logType;
        LogMessageFormat = logMessageFormat;
        Message = message;
    }
    
    protected Log(string logString)
    {
        LogNumber = LogStringParser.ParseLogNumber(logString);
        CreationTime = LogStringParser.ParseCreationTime(logString);
        LogType = LogStringParser.ParseLogType(logString);
        LogMessageFormat = LogStringParser.ParseLogMessageFormat(logString);
        Message = LogStringParser.ParseLogMessage(logString);
    }

    public abstract string GetCreationTimeStamp();
    
    public static string LogDelimiter()
    {
        // WARNING: MUST NOT BE SET TO ANY REGEX OPERATORS,
        // SUCH AS | or . or ^ or $ etc., OTHERWISE EVERYTHING BREAKS.
        // USING ESCAPED REGEX OPERATORS IS NOT AN OPTION. 
        return "  ";
    }

    /// <summary>
    /// Describes a pattern for a valid log string of the form: "LogNumber {LogDelimiter} CreationTimeStamp {LogDelimiter} LogType {LogDelimiter} LogMessageFormat {LogDelimiter} Message"
    /// A valid string could be: "36 | 12/05/2027 11.56.45 | Vacuuming | Plaintext | This is a vacuuming message"
    /// </summary>
    /// <returns></returns>
    // 
    private static string ValidLogStringPattern()
    {
        return
            $"^\\d+{LogDelimiter()}\\d+\\/\\d+\\/\\d+ \\d+\\.\\d+\\.\\d+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}.*$";
    }

    public static bool IsValidLogString(string logString)
    {
        return Regex.IsMatch(logString, Log.ValidLogStringPattern());
    }

    
}