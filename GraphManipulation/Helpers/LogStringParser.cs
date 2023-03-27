using System.Globalization;
using GraphManipulation.Logging;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Helpers;

public static class LogStringParser
{
    private static string GetRelevantPartFromString(string logString, int index)
    {
        if (!BaseLogger.IsValidLogString(logString))
        {
            throw new LogStringParserException("Log string is not valid: " + logString);
        }

        return logString.Split(BaseLogger.LogDelimiter())[index];
    }

    public static int ParseLogNumber(string logString)
    {
        var logNumberString = GetRelevantPartFromString(logString, 0);

        if (int.TryParse(logNumberString, out var result))
        {
            return result;
        }

        throw new LogStringParserException("Could not parse log number: " + logNumberString);
    }

    public static DateTime ParseCreationTime(string logString)
    {
        var creationTimeString = GetRelevantPartFromString(logString, 1);

        return DateTime.ParseExact(creationTimeString, "dd/MM/yyyy HH.mm.ss", CultureInfo.InvariantCulture);
    }

    public static LogType ParseLogType(string logString)
    {
        var logTypeString = GetRelevantPartFromString(logString, 2);

        if (Enum.TryParse(typeof(LogType), logTypeString, out var result))
        {
            return (LogType)result!;
        }

        throw new LogStringParserException("Could not parse log type: " + logTypeString);
    }

    public static LogMessageFormat ParseLogMessageFormat(string logString)
    {
        var logMessageFormatString = GetRelevantPartFromString(logString, 3);

        if (Enum.TryParse(typeof(LogMessageFormat), logMessageFormatString, out var result))
        {
            return (LogMessageFormat)result!;
        }

        throw new LogStringParserException("Could not parse log message format: " + logMessageFormatString);
    }

    public static string ParseLogMessage(string logString)
    {
        var logMessage = GetRelevantPartFromString(logString, 4);

        return logMessage;
    }
}

public class LogStringParserException : Exception
{
    public LogStringParserException(string message) : base(message)
    {
    }
}