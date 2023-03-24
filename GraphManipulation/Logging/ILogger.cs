using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogger
{
    public void Append(ILog log);
    public IOrderedEnumerable<ILog> Read(LoggerReadOptions options);
    public int ServeNextLogNumber();
}