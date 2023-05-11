﻿using GraphManipulation.Logging;
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

    private void AppendLogEntry(Operation operation, LogType logType = LogType.Metadata)
    {
        _logger.Append(new MutableLog(logType, operation, LogMessageFormat.Plaintext, operation.ToString()));
    }
    
    private static Dictionary<string, string>? GetParameters(object? obj)
    {
        return obj is null ? null : AnonymousObjectToDict(obj);
    }
    
    private static Dictionary<string, string> AnonymousObjectToDict(object obj)
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

    public void LogCreate(string key, object parameters)
    {
        AppendLogEntry(new Create(_type, key, GetParameters(parameters)));
    }

    public void LogSet(string key, object parameters)
    {
        AppendLogEntry(new Set(_type, key, GetParameters(parameters)));
    }

    public void LogExecute(string key, object parameters)
    {
        AppendLogEntry(new Execute(_type, key, GetParameters(parameters)));
    }
}