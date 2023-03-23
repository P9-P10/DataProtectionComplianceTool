using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class LoggerReadOptions
{
    public readonly NumberRange LogNumbersRange;
    public readonly TimeRange LogTimeRange;
    public readonly List<LogType> LogTypes;
    public readonly List<LogMessageFormat> LogMessageFormats;
    
    public LoggerReadOptions(NumberRange? logNumberRange = null, TimeRange? timeRange = null, List<LogType>? logTypes = null,
        List<LogMessageFormat>? logMessageFormats = null)
    {
        LogNumbersRange = logNumberRange ?? new NumberRange(int.MinValue, int.MaxValue);
        LogTimeRange = timeRange ?? new TimeRange(DateTime.MinValue, DateTime.MaxValue);
        LogTypes = logTypes ?? Enum.GetValues<LogType>().ToList();
        LogMessageFormats = logMessageFormats ?? Enum.GetValues<LogMessageFormat>().ToList();
    }
}