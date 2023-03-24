using System.Globalization;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Helpers;

public static class LogStringParser
{
    public static ILog Parse(string logString)
    {
        var splitLogString = logString.Split(Log.LogDelimiter());
        
        var logNumber = ParseLogNumber(splitLogString[0]);
        var creationTime = ParseCreationTime(splitLogString[1]);
        var logType = ParseLogType(splitLogString[2]);
        var logMessageFormat = ParseLogMessageFormat(splitLogString[3]);
        var message = splitLogString[4];

        return new Log(logNumber, creationTime, logType, logMessageFormat, message);
    }
    
    private static int ParseLogNumber(string logNumberString)
    {
        if (int.TryParse(logNumberString, out var result))
        {
            return result;
        }

        throw new LogException("Could not parse log id: " + logNumberString);
    }

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
}