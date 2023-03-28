using System.Text.RegularExpressions;
using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public abstract class Logger : ILogger
{
    private readonly ConfigManager _configManager;
    private readonly ILogFileSegmenter _logFileSegmenter;
    private int _logNumber;
    private string _currentLogFileName;

    protected Logger(ConfigManager configManager, ILogFileSegmenter logFileSegmenter)
    {
        _configManager = configManager;
        
        if (string.IsNullOrEmpty(GetLogFolderPath()))
        {
            throw new LoggerException("Value for log path must be set in config file");
        }
        
        _logFileSegmenter = logFileSegmenter;
        _currentLogFileName = LoadCurrentLogFileName();
        _logNumber = LoadCurrentLogNumber();
    }

    public void Append(IMutableLog mutableLog)
    {
        var log = CreateLog(mutableLog);

        if (string.IsNullOrEmpty(_currentLogFileName) || _logFileSegmenter.IsCriteriaMetForSegmentation(log))
        {
            ResetLogNumber();
            log = CreateLog(mutableLog);
            _currentLogFileName = log.GetCreationTimeStamp();
            SaveCurrentLogFileName(_currentLogFileName);
        }

        AppendLogToFile(log);
    }
    protected abstract void AppendLogToFile(ILog log);
    
    public abstract IOrderedEnumerable<ILog> Read(ILogConstraints constraints);
    protected abstract ILog CreateLog(IMutableLog mutableLog);
    protected abstract ILog CreateLog(LogType logType, LogMessageFormat logMessageFormat, string message);

    protected string GetLogFolderPath() => _configManager.GetValue("LogFolderPath");
    protected string GetLogFilePath() => GetLogFolderPath() + _currentLogFileName;
    private string GetCurrentLogFileNamePath() => GetLogFolderPath() + "current_log_file_name";

    private string LoadCurrentLogFileName()
    {
        return File.Exists(GetCurrentLogFileNamePath()) ? File.ReadAllText(GetCurrentLogFileNamePath()) : "";
    }

    private void SaveCurrentLogFileName(string logFileName)
    {
        File.WriteAllText(GetCurrentLogFileNamePath(), logFileName);
    }

    private void ResetLogNumber()
    {
        _logNumber = 1;
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