namespace GraphManipulation.Logging;

public interface IMutableLog
{
    public LogType LogType { get; set; }
    public LogMessageFormat LogMessageFormat { get; set; }
    public string Message { get; set; }
}