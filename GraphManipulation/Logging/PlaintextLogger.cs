using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

// TODO: Lav en ny log fil per [indsæt segmenterings faktor], hav unikt index i hver fil (dato + tidspunkt + index = unik)

public class PlaintextLogger : Logger
{
    public PlaintextLogger(ConfigManager configManager/*, ILogFileSegmenter logFileSegmenter*/) 
        : base(configManager/*, logFileSegmenter*/)
    {
    }

    protected override ILog CreateLog(IMutableLog mutableLog)
    {
        return CreateLog(mutableLog.LogType, mutableLog.LogMessageFormat, mutableLog.Message);
    }

    protected override ILog CreateLog(LogType logType, LogMessageFormat logMessageFormat, string message)
    {
        return new Log(ServeNextLogNumber(), DateTime.Now, logType, logMessageFormat, message);
    }

    protected override void AppendLogToFile(ILog log)
    {
        var writer = File.AppendText(GetLogFilePath());
        writer.WriteLine(log.ToString());
        writer.Flush();
        writer.Close();
    }

    public override IOrderedEnumerable<ILog> Read(ILogConstraints constraints)
    {
        var logs = File.ReadLines(GetLogFilePath()).Select(s => new Log(s));
        return constraints.ApplyConstraintsToLogs(logs);
    }

    protected override int LoadCurrentLogNumber()
    {
        var filePath = GetLogFilePath();
    
        if (!File.Exists(filePath))
        {
            return 1;
        }
    
        var lastLogString = File.ReadLines(filePath).LastOrDefault();
    
        if (lastLogString is null)
        {
            return 1;
        }
    
        if (Log.IsValidLogString(lastLogString))
        {
            return new Log(lastLogString).LogNumber + 1;
        }
    
        throw new LoggerException("Log could not be parsed: " + lastLogString);
    }
}