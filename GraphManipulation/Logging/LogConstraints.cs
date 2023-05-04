using GraphManipulation.Helpers;

namespace GraphManipulation.Logging;

public class LogConstraints : ILogConstraints
{
    public LogConstraints(NumberRange? logNumberRange = null, TimeRange? timeRange = null,
        IEnumerable<LogType>? logTypes = null,
        IEnumerable<string>? subjects = null,
        IEnumerable<LogMessageFormat>? logMessageFormats = null)
    {
        LogNumbersRange = logNumberRange ?? new NumberRange(int.MinValue, int.MaxValue);
        LogTimeRange = timeRange ?? new TimeRange(DateTime.MinValue, DateTime.MaxValue);
        LogTypes = logTypes ?? Enum.GetValues<LogType>().ToList();
        Subjects = subjects ?? Array.Empty<string>();
        LogMessageFormats = logMessageFormats ?? Enum.GetValues<LogMessageFormat>().ToList();
    }

    private NumberRange LogNumbersRange { get; }
    private TimeRange LogTimeRange { get; }
    private IEnumerable<LogType> LogTypes { get; }
    private IEnumerable<string> Subjects { get; }
    private IEnumerable<LogMessageFormat> LogMessageFormats { get; }

    public IOrderedEnumerable<ILog> ApplyConstraintsToLogs(IEnumerable<ILog> logs)
    {
        return logs
            .Where(log => LogNumbersRange.NumberWithinRange(log.LogNumber) &&
                          LogTimeRange.DateTimeWithinRange(log.CreationTime) &&
                          LogTypes.Contains(log.LogType) &&
                          (!Subjects.Any() || Subjects.Contains(log.Subject)) &&
                          LogMessageFormats.Contains(log.LogMessageFormat))
            .OrderBy(log => log.LogNumber);
    }

    public bool Equals(LogConstraints other)
    {
        return other.LogNumbersRange.Equals(LogNumbersRange) &&
               other.LogTimeRange.Equals(LogTimeRange) &&
               other.LogTypes.Equals(LogTypes) &&
               other.Subjects.Equals(Subjects) &&
               other.LogMessageFormats.Equals(LogMessageFormats);
    }
}