using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public abstract class BaseLogger : ILogger
{
    private readonly ConfigManager _configManager;
    
    public BaseLogger(ConfigManager configManager)
    {
        _configManager = configManager;

        var path = GetLogPath();

        if (string.IsNullOrEmpty(path))
        {
            throw new LoggerException("Value for LogPath must be set in config file");
        }
    }
    
    protected string GetLogPath() => _configManager.GetValue("LogPath");

    public abstract void Append(ILog log);

    public abstract ILog Read();

    public abstract List<ILog> ReadAll();
}

public class LoggerException : Exception
{
    public LoggerException(string message) : base(message)
    {
    }
}