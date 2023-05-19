using System.Globalization;

namespace GraphManipulation.Logging;

public static class LogStringParser
{
    private static string GetRelevantPartFromString(string logString, int index)
    {
        if (!Log.IsValidLogString(logString))
        {
            throw new LogStringParserException("Log string is not valid: " + logString);
        }

        return logString.Split(Log.LogDelimiter())[index];
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

        try
        {
            return DateTime.ParseExact(creationTimeString, Log.CreationTimeStampFormat(), CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            throw new LogStringParserException("Could not parse creation time: " + creationTimeString);
        }
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

    public static string ParseSubject(string logString)
    {
        var subject = GetRelevantPartFromString(logString, 3);

        return subject;
    }

    public static LogMessageFormat ParseLogMessageFormat(string logString)
    {
        var logMessageFormatString = GetRelevantPartFromString(logString, 4);

        if (Enum.TryParse(typeof(LogMessageFormat), logMessageFormatString, out var result))
        {
            return (LogMessageFormat)result!;
        }

        throw new LogStringParserException("Could not parse log message format: " + logMessageFormatString);
    }

    public static string ParseLogMessage(string logString)
    {
        var logMessage = GetRelevantPartFromString(logString, 5);

        return logMessage;
    }
}