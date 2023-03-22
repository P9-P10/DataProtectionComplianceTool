using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogger
{
    public void Append(ILog log);
    public ILog Read();
    public List<ILog> ReadAll();
}