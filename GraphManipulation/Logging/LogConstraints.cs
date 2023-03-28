using GraphManipulation.Helpers;
using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public class LogConstraints : ILogConstraints
{
    public LogConstraints(/*NumberRange? logNumberRange = null, */TimeRange? timeRange = null,
        List<LogType>? logTypes = null,
        List<LogMessageFormat>? logMessageFormats = null)
    {
        // LogNumbersRange = logNumberRange ?? new NumberRange(int.MinValue, int.MaxValue);
        LogTimeRange = timeRange ?? new TimeRange(DateTime.MinValue, DateTime.MaxValue);
        LogTypes = logTypes ?? Enum.GetValues<LogType>().ToList();
        LogMessageFormats = logMessageFormats ?? Enum.GetValues<LogMessageFormat>().ToList();
    }

    // public NumberRange LogNumbersRange { get; }
    public TimeRange LogTimeRange { get; }
    public List<LogType> LogTypes { get; }
    public List<LogMessageFormat> LogMessageFormats { get; }

    public IOrderedEnumerable<ILog> ApplyConstraintsToLogs(IEnumerable<ILog> logs)
    {
        return logs
            .Where(log => /*LogNumbersRange.NumberWithinRange(log.LogNumber) &&*/
                          LogTimeRange.DateTimeWithinRange(log.CreationTime) &&
                          LogTypes.Contains(log.LogType) &&
                          LogMessageFormats.Contains(log.LogMessageFormat))
            .OrderBy(log => log.LogNumber);
    }
}