using GraphManipulation.Logging;
using GraphManipulation.Logging.Operations;

namespace GraphManipulation.Decorators;

/// <summary>
/// This is a parent class for decorators responsible for logging.
/// It provides methods for creating log messages.
/// </summary>
public class LoggingDecorator
{
    private readonly ILogger _logger;
    private readonly string _type;

    public LoggingDecorator(ILogger logger, string type)
    {
        _logger = logger;
        _type = type;
    }

    private void AppendLogEntry(Operation operation)
    {
        _logger.Append(new MutableLog(LogType.Metadata, operation, LogMessageFormat.Plaintext, operation.ToString()));
    }


    private Dictionary<string, string>? GetParameters(object? obj)
    {
        return obj is null ? null : AnonymousObjectToDict(obj);
    }
    private Dictionary<string, string> AnonymousObjectToDict(object obj)
    {
        return obj
            .GetType()
            .GetProperties()
            .ToDictionary(x => x.Name, x => x.GetValue(obj, null).ToString());
    }

    public void LogDelete(string key)
    {
        AppendLogEntry(new Delete(_type, key));
    }

    public void LogUpdate(string key, object parameters)
    {
        AppendLogEntry(new Update(_type, key, GetParameters(parameters)));
    }

    public void LogAdd(string key, object parameters)
    {
        AppendLogEntry(new Add(_type, key, GetParameters(parameters)));
    }

    public void LogSet(string key, object parameters)
    {
        AppendLogEntry(new Set(_type, key, GetParameters(parameters)));
    }
}