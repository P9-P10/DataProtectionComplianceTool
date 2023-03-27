using System.Text.RegularExpressions;
using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public abstract class Logger : ILogger
{
    private readonly ConfigManager _configManager;
    private int _logNumber;

    protected Logger(ConfigManager configManager)
    {
        _configManager = configManager;

        if (string.IsNullOrEmpty(GetLogPath()))
        {
            throw new LoggerException("Value for log path must be set in config file");
        }

        _logNumber = GetCurrentLogNumberFromFile();
    }

    public abstract void Append(ILog log);
    public abstract IOrderedEnumerable<ILog> Read(ILoggerConstraints constraints);
    public abstract ILog CreateLog(LogType logType, LogMessageFormat logMessageFormat, string message);

    protected string GetLogPath()
    {
        return _configManager.GetValue("LogPath");
    }

    protected int ServeNextLogNumber()
    {
        return _logNumber++;
    }

    protected abstract int GetCurrentLogNumberFromFile();
}

public class LoggerException : Exception
{
    public LoggerException(string message) : base(message)
    {
    }
}