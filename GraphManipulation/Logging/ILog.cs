namespace GraphManipulation.Logging.Logs;

public interface ILog
{
    public int LogNumber { get; }
    public DateTime CreationTime { get; }
    public LogType LogType { get; }
    public LogMessageFormat LogMessageFormat { get; }
    public string Message { get; }

    public string ToString();
    public string GetCreationTimeStamp();
}