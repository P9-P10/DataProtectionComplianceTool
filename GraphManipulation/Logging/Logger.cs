using GraphManipulation.Helpers;

namespace GraphManipulation.Logging;

public abstract class Logger : ILogger
{
    private readonly IConfigManager _configManager;
    private int _logNumber;

    protected Logger(IConfigManager configManager)
    {
        _configManager = configManager;

        if (string.IsNullOrEmpty(GetLogFilePath()))
        {
            throw new LoggerException("Value for log path must be set in config file");
        }
        
        _logNumber = LoadCurrentLogNumber();
    }

    public void Append(IMutableLog mutableLog)
    {
        var log = CreateLog(mutableLog);

        AppendLogToFile(log);
    }

    public abstract IEnumerable<ILog> Read(ILogConstraints constraints);
    protected abstract void AppendLogToFile(ILog log);
    protected abstract ILog CreateLog(IMutableLog mutableLog);
    protected abstract ILog CreateLog(LogType logType, string subject, LogMessageFormat logMessageFormat, string message);

    protected string GetLogFilePath()
    {
        return _configManager.GetValue("LogPath");
    }

    protected int ServeNextLogNumber()
    {
        return _logNumber++;
    }

    protected abstract int LoadCurrentLogNumber();
}

public class LoggerException : Exception
{
    public LoggerException(string message) : base(message)
    {
    }
}