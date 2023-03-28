using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

// TODO: Lav en ny log fil per [indsæt segmenterings faktor], hav unikt index i hver fil (dato + tidspunkt + index = unik)

public class PlaintextLogger : Logger
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
        return constraints.ApplyConstraintsToLogs(logs);
    }

    public override void CreateAndAppendLog(LogType logType, LogMessageFormat logMessageFormat, string message)
    {
        Append(CreateLog(logType, logMessageFormat, message));
    }

    protected override int GetCurrentLogNumberFromFile()
    {
        var path = GetLogPath();

        if (!File.Exists(path))
        {
            var openWrite = File.OpenWrite(path);
            openWrite.Dispose();
        }

        var lastLogString = File.ReadLines(path).LastOrDefault();

        if (lastLogString is null)
        {
            return 1;
        }

        if (Log.IsValidLogString(lastLogString))
        {
            return new PlaintextLog(lastLogString).LogNumber + 1;
        }

        throw new LoggerException("Log could not be parsed: " + lastLogString);
    }
}