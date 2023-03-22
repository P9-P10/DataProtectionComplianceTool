using System.Globalization;
using System.Text.RegularExpressions;

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
    // public readonly long LogId;
    public readonly DateTime CreationTime;
    public readonly LogType LogType;
    public readonly LogMessageFormat LogMessageFormat;
    public readonly string Message;
    
    public Log(LogType logType, LogMessageFormat logMessageFormat, string message)
    {
        // LogId = logId;
        CreationTime = DateTime.Now;
        LogType = logType;
        LogMessageFormat = logMessageFormat;
        Message = message;
    }

    public Log(string logString)
    {
        if (!IsValidLogString(logString))
        {
            throw new LogException("Log string is not valid: " + logString);
        }
        
        var splitLogString = logString.Split(LogDelimiter());

        // LogId = ParseLogId(splitLogString[0]);
        CreationTime = ParseCreationTime(splitLogString[1]);
        LogType = ParseLogType(splitLogString[2]);
        LogMessageFormat = ParseLogMessageFormat(splitLogString[3]);
        Message = splitLogString[4];
    }

    // private static long ParseLogId(string logIdString)
    // {
    //     if (long.TryParse(logIdString, out var result))
    //     {
    //         return result;
    //     }
    //     
    //     throw new LogException("Could not parse log id: " + logIdString);
    // }

    private static DateTime ParseCreationTime(string creationTimeString)
    {
        return DateTime.ParseExact(creationTimeString, "dd/MM/yyyy HH.mm.ss", CultureInfo.InvariantCulture);
    }

    private static LogType ParseLogType(string logTypeString)
    {
        if (Enum.TryParse(enumType: typeof(LogType), value: logTypeString, result: out var result))
        {
            return (LogType)result!;
        }

        throw new LogException("Could not parse log type: " + logTypeString);
    }

    private static LogMessageFormat ParseLogMessageFormat(string logMessageFormatString)
    {
        if (Enum.TryParse(enumType: typeof(LogMessageFormat), value: logMessageFormatString, result: out var result))
        {
            return (LogMessageFormat)result!;
        }
        
        throw new LogException("Could not parse log message format: " + logMessageFormatString);
    }

    public static string LogDelimiter() => " <<>> ";

    private static string ValidLogStringPattern() => $"^\\d+{LogDelimiter()}\\d+\\/\\d+\\/\\d+ \\d+\\.\\d+\\.\\d+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}.+$";

    public static bool IsValidLogString(string logString)
    {
        return Regex.IsMatch(logString, ValidLogStringPattern());
    }

    public string LogToString()
    {
        return GetCreationTimeStamp() + LogDelimiter() + 
               LogType + LogDelimiter() + 
               LogMessageFormat + LogDelimiter() + 
               Message;
    }

    public string GetCreationTimeStamp()
    {
        return CreationTime.ToShortDateString() + " " + CreationTime.ToLongTimeString();
    }
}

public class LogException : Exception
{
    public LogException(string message) : base(message)
    {
    }
}



