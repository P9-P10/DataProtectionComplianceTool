using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class PlaintextLogger : BaseLogger
{
    private int _lineNumber;
    
    public PlaintextLogger(ConfigManager configManager) : base(configManager)
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
            _lineNumber = 1;
        }
        else if (Log.IsValidLogString(lastLine))
        {
            _lineNumber = new Log(lastLine).LogNumber!.Value + 1;
        }
        else
        {
            throw new LoggerException("Line number could not be parsed: " + lastLine);
        }
    }
    
    public override void Append(ILog log)
    {
        var writer = File.AppendText(GetLogPath());
        log.SetLogNumber(_lineNumber);
        writer.WriteLine(log.LogToString());
        _lineNumber++;
        writer.Dispose();
    }

    public override IEnumerable<Log> Read(LoggerReadOptions options)
    {
        return File.ReadLines(GetLogPath())
            .Select(s => new Log(s))
            .Where(log => options.LogNumbersRange.NumberWithinRange(log.LogNumber!.Value) &&
                          options.LogTimeRange.DateTimeWithinRange(log.CreationTime) &&
                          options.LogTypes.Contains(log.LogType) &&
                          options.LogMessageFormats.Contains(log.LogMessageFormat));
    }
}

