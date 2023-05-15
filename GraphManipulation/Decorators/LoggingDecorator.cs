using GraphManipulation.Logging;
using GraphManipulation.Logging.Operations;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Decorators;

/// <summary>
/// This is a parent class for decorators responsible for logging.
/// It provides methods for creating log messages.
/// </summary>
public class LoggingDecorator<TKey, TValue> where TValue : Entity<TKey>
{
    private readonly ILogger _logger;
    public LoggingDecorator(ILogger logger)
    {
        _logger = logger;
    }

    private void AppendLogEntry(
        Operation<TKey, TValue> operation, 
        LogType logType = LogType.Metadata, 
        LogMessageFormat logMessageFormat = LogMessageFormat.Plaintext)
    {
        AppendLogEntry(operation, logType, logMessageFormat, operation.ToString());
    }

    private void AppendLogEntry(
        Operation<TKey, TValue> operation, 
        LogType logType, 
        LogMessageFormat logMessageFormat,
        string message)
    {
        _logger.Append(new MutableLog(logType, operation.Key!.ToString()!, logMessageFormat, message));
    }

    public void LogDelete(TKey key)
    {
        AppendLogEntry(new Delete<TKey, TValue>(key));
    }

    public void LogUpdate(TKey key, TValue value)
    {
        AppendLogEntry(new Update<TKey, TValue>(key, value));
    }

    public void LogCreate(TKey key)
    {
        AppendLogEntry(new Create<TKey, TValue>(key));
    }

    public void LogExecute(TKey key)
    {
        AppendLogEntry(new Execute<TKey, TValue>(key), LogType.Vacuuming);
    }
}