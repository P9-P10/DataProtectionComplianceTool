using System.Text.RegularExpressions;
using GraphManipulation.Extensions;
using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class PlaintextLogger : BaseLogger
{
    public PlaintextLogger(ConfigManager configManager) : base(configManager)
    {
    }

    public override ILog CreateLog(LogType logType, LogMessageFormat logMessageFormat, string message)
    {
        return new PlaintextLog(ServeNextLogNumber(), DateTime.Now, logType, logMessageFormat, message);
    }

    public override void Append(ILog log)
    {
        var writer = File.AppendText(GetLogPath());
        writer.WriteLine(log.ToString());
        writer.Dispose();
    }

    public override IOrderedEnumerable<ILog> Read(ILoggerConstraints constraints)
    {
        var logs = File.ReadLines(GetLogPath()).Select(s => new PlaintextLog(s));
        return constraints.Apply(logs);
        
    }

    protected override int GetCurrentLogNumberFromFile()
    {
        var path = GetLogPath();

        if (!File.Exists(path))
        {
            var openWrite = File.OpenWrite(path);
            openWrite.Dispose();
        }

        var lastLine = File.ReadLines(path).LastOrDefault();

        if (lastLine is null)
        {
            return 1;
        }

        if (IsValidLogString(lastLine))
        {
            return new PlaintextLog(lastLine).LogNumber + 1;
        }

        throw new LoggerException("Line number could not be parsed: " + lastLine);
    }

    private class PlaintextLog : ILog
    {
        public int LogNumber { get; }
        public DateTime CreationTime { get; }
        public LogType LogType { get; }
        public LogMessageFormat LogMessageFormat { get; }
        public string Message { get; }
    
        public PlaintextLog(int logNumber, DateTime creationTime, LogType logType, LogMessageFormat logMessageFormat, string message)
        {
            LogNumber = logNumber;
            CreationTime = creationTime;
            LogType = logType;
            LogMessageFormat = logMessageFormat;
            Message = message;
        }
    
        public PlaintextLog(string logString)
        {
            LogNumber = LogStringParser.ParseLogNumber(logString);
            CreationTime = LogStringParser.ParseCreationTime(logString);
            LogType = LogStringParser.ParseLogType(logString);
            LogMessageFormat = LogStringParser.ParseLogMessageFormat(logString);
            Message = LogStringParser.ParseLogMessage(logString);
        }

        public override string ToString()
        {
            return LogNumber + LogDelimiter() +
                   GetCreationTimeStamp() + LogDelimiter() +
                   LogType + LogDelimiter() +
                   LogMessageFormat + LogDelimiter() +
                   Message;
        }
    
        public string GetCreationTimeStamp()
        {
            return CreationTime.ToShortDateString() + " " + CreationTime.ToLongTimeString();
        }
    
        protected bool Equals(ILog other)
        {
            return CreationTime.Equals(other.CreationTime) && LogType == other.LogType &&
                   LogMessageFormat == other.LogMessageFormat && Message == other.Message;
        }
    
        public override int GetHashCode()
        {
            return HashCode.Combine(LogNumber, CreationTime, (int)LogType, (int)LogMessageFormat, Message);
        }
    }
}