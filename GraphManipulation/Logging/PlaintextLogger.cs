using GraphManipulation.Helpers;

namespace GraphManipulation.Logging;

public class PlaintextLogger : Logger
{
    public PlaintextLogger(IConfigManager configManager)
        : base(configManager)
    {
    }

    protected override ILog CreateLog(IMutableLog mutableLog)
    {
        return CreateLog(mutableLog.LogType, mutableLog.Subject, mutableLog.LogMessageFormat, mutableLog.Message);
    }

    protected override ILog CreateLog(LogType logType, string subject, LogMessageFormat logMessageFormat, string message)
    {
        return new Log(ServeNextLogNumber(), DateTime.Now, logType, subject, logMessageFormat, message);
    }

    protected override void AppendLogToFile(ILog log)
    {
        var writer = File.AppendText(GetLogFilePath());
        writer.WriteLine(log.ToString());
        writer.Flush();
        writer.Close();
    }

    public override IEnumerable<ILog> Read(ILogConstraints constraints)
    {
        var logs = File.ReadLines(GetLogFilePath()).Select(s => new Log(s)).ToList();
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