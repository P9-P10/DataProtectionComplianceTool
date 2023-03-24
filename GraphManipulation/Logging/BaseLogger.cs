using System.Text.RegularExpressions;
using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public abstract class BaseLogger : ILogger
{
    private readonly ConfigManager _configManager;
    private int _logNumber;

    protected BaseLogger(ConfigManager configManager)
    {
        _configManager = configManager;

        if (string.IsNullOrEmpty(GetLogPath()))
        {
            throw new LoggerException("Value for log path must be set in config file");
        }

        _logNumber = GetCurrentLogNumberFromFile();
    }
    
    protected string GetLogPath() => _configManager.GetValue("LogPath");

    public abstract void Append(ILog log);
    public abstract IOrderedEnumerable<ILog> Read(LoggerReadOptions options);
    public abstract ILog CreateLog(LogType logType, LogMessageFormat logMessageFormat, string message);

    protected int ServeNextLogNumber()
    {
        return _logNumber++;
    }

    protected abstract int GetCurrentLogNumberFromFile();
    
    public static string LogDelimiter() => " <<>> ";
    
    private static string ValidLogStringPattern() =>
        $"^\\d+{LogDelimiter()}\\d+\\/\\d+\\/\\d+ \\d+\\.\\d+\\.\\d+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}[a-zA-Z0-9_ ]+{LogDelimiter()}.*$";
    
    public static bool IsValidLogString(string logString)
    {
        return Regex.IsMatch(logString, ValidLogStringPattern());
    }
}

public class LoggerException : Exception
{
    public LoggerException(string message) : base(message)
    {
    }
}