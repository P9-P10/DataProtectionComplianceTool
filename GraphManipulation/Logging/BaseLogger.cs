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

    public int ServeNextLogNumber()
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