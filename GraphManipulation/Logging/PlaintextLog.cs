using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class PlaintextLog : Log
{
    public PlaintextLog(int logNumber, DateTime creationTime, LogType logType, LogMessageFormat logMessageFormat,
        string message) : base(logNumber, creationTime, logType, logMessageFormat, message)
    {
    }

    public PlaintextLog(string logString) : base(logString)
    {
    }

    public override string ToString()
    {
        return LogNumber + LogDelimiter() +
               GetCreationTimeStamp() + LogDelimiter() +
               LogType + LogDelimiter() +
               LogMessageFormat + LogDelimiter() +
               Message;
    }

    public override string GetCreationTimeStamp()
    {
        return CreationTime.ToShortDateString() + " " + CreationTime.ToLongTimeString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LogNumber, CreationTime, (int)LogType, (int)LogMessageFormat, Message);
    }
}