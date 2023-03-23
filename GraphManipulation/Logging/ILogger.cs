using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogger
{
    public void Append(ILog log);
    public IEnumerable<Log> Read(LoggerReadOptions options);
}