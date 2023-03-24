using System.Globalization;
using System.Text.RegularExpressions;
using GraphManipulation.Extensions;
using GraphManipulation.Helpers;

namespace GraphManipulation.Logging.Logs;

public enum LogType
{
    SchemaChange,
    Vacuuming,
    Metadata
}

public enum LogMessageFormat
{
    Json,
    Plaintext,
    Turtle
}

public class LogException : Exception
{
    public LogException(string message) : base(message)
    {
    }
}