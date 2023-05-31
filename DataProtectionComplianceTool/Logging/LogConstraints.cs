using GraphManipulation.Utility;

namespace GraphManipulation.Logging;

public class LogConstraints : ILogConstraints
{
    public LogConstraints(NumberRange? logNumberRange = null, TimeRange? timeRange = null,
        IEnumerable<LogType>? logTypes = null,
        IEnumerable<string>? subjects = null,
        IEnumerable<LogMessageFormat>? logMessageFormats = null, int limit = 100)
    {
        LogNumbersRange = logNumberRange ?? new NumberRange(0, int.MaxValue);
        LogTimeRange = timeRange ?? new TimeRange(DateTime.MinValue, DateTime.MaxValue);
        LogTypes = logTypes ?? Enum.GetValues<LogType>().ToList();
        Subjects = subjects ?? Array.Empty<string>();
        LogMessageFormats = logMessageFormats ?? Enum.GetValues<LogMessageFormat>().ToList();
        Limit = limit;
    }

    public NumberRange LogNumbersRange { get; }
    public TimeRange LogTimeRange { get; }
    public IEnumerable<LogType> LogTypes { get; }
    public IEnumerable<string> Subjects { get; }
    public IEnumerable<LogMessageFormat> LogMessageFormats { get; }
    public int Limit { get; }

    public IEnumerable<ILog> ApplyConstraintsToLogs(IEnumerable<ILog> logs)
    {
        return logs
            .Where(log => LogNumbersRange.NumberWithinRange(log.LogNumber) &&
                          LogTimeRange.DateTimeWithinRange(log.CreationTime) &&
                          LogTypes.Contains(log.LogType) &&
                          (!Subjects.Any() || Subjects.Contains(log.Subject)) &&
                          LogMessageFormats.Contains(log.LogMessageFormat))
            .OrderBy(log => log.LogNumber)
            .TakeLast(Limit);
    }

    public bool Equals(LogConstraints other)
    {
        var limitEquality = other.Limit.Equals(Limit);
        var numberEquality = other.LogNumbersRange.Equals(LogNumbersRange);
        var timeEquality = other.LogTimeRange.Equals(LogTimeRange);
        var typeEquality = !other.LogTypes.Except(LogTypes).Any();
        var subjectEquality = !other.Subjects.Except(Subjects).Any();
        var formatEquality = !other.LogMessageFormats.Except(LogMessageFormats).Any();

        return limitEquality && numberEquality && timeEquality && typeEquality && subjectEquality && formatEquality;
    }
}