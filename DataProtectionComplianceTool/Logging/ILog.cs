namespace GraphManipulation.Logging;

public interface ILog
{
    public int LogNumber { get; }
    public DateTime CreationTime { get; }
    public LogType LogType { get; }
    public string Subject { get; }
    public LogMessageFormat LogMessageFormat { get; }
    public string Message { get; }

    public string ToString();
    public string GetCreationTimeStamp();
}