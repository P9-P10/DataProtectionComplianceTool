using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Extensions;

public static class StringToLogExtension
{
    public static ILog StringToLog(this string logString)
    {
        if (!Log.IsValidLogString(logString))
        {
            throw new LogException("Log string is not valid: " + logString);
        }

        return LogStringParser.Parse(logString);
    }
}