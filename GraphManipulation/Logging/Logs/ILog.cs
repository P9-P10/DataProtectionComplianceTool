namespace GraphManipulation.Logging.Logs;

public interface ILog
{
    public string LogToString();
    public void SetLogNumber(int logNumber);
}