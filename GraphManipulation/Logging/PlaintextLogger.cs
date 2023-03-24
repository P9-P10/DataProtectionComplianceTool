using GraphManipulation.Extensions;
using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class PlaintextLogger : BaseLogger
{
    public PlaintextLogger(ConfigManager configManager) : base(configManager)
    {
    }

    public override void Append(ILog log)
    {
        var writer = File.AppendText(GetLogPath());
        writer.WriteLine(log.LogToString());
        writer.Dispose();
    }

    public override IOrderedEnumerable<ILog> Read(LoggerReadOptions options)
    {
        return File.ReadLines(GetLogPath())
            .Select(s => s.StringToLog())
            .Where(log => options.LogNumbersRange.NumberWithinRange(log.LogNumber) &&
                          options.LogTimeRange.DateTimeWithinRange(log.CreationTime) &&
                          options.LogTypes.Contains(log.LogType) &&
                          options.LogMessageFormats.Contains(log.LogMessageFormat))
            .OrderBy(log => log.LogNumber);
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

        if (Log.IsValidLogString(lastLine))
        {
            return lastLine.StringToLog().LogNumber + 1;
        }

        throw new LoggerException("Line number could not be parsed: " + lastLine);
    }
}