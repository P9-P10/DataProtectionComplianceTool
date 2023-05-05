using GraphManipulation.Helpers;

namespace GraphManipulation.Logging;

public class LogConstraints : ILogConstraints
{
    public LogConstraints(NumberRange? logNumberRange = null, TimeRange? timeRange = null,
        IEnumerable<LogType>? logTypes = null,
        IEnumerable<string>? subjects = null,
        IEnumerable<LogMessageFormat>? logMessageFormats = null, int limit = 100)
    {
        LogNumbersRange = logNumberRange ?? new NumberRange(int.MinValue, int.MaxValue);
        LogTimeRange = timeRange ?? new TimeRange(DateTime.MinValue, DateTime.MaxValue);
        LogTypes = logTypes ?? Enum.GetValues<LogType>().ToList();
        Subjects = subjects ?? Array.Empty<string>();
        LogMessageFormats = logMessageFormats ?? Enum.GetValues<LogMessageFormat>().ToList();
        Limit = limit;
    }

    private NumberRange LogNumbersRange { get; }
    private TimeRange LogTimeRange { get; }
    private IEnumerable<LogType> LogTypes { get; }
    private IEnumerable<string> Subjects { get; }
    private IEnumerable<LogMessageFormat> LogMessageFormats { get; }
    private int Limit { get; }

    public IEnumerable<ILog> ApplyConstraintsToLogs(IEnumerable<ILog> logs)
    {
        return logs
            .Where(log => LogNumbersRange.NumberWithinRange(log.LogNumber) &&
                          LogTimeRange.DateTimeWithinRange(log.CreationTime) &&
                          LogTypes.Contains(log.LogType) &&
                          (!Subjects.Any() || Subjects.Contains(log.Subject)) &&
                          LogMessageFormats.Contains(log.LogMessageFormat))
            .OrderBy(log => log.LogNumber)
            .Take(Limit);
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