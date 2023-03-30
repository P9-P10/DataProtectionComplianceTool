using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogger
{
    public void Append(IMutableLog mutableLog);
    public IOrderedEnumerable<ILog> Read(ILogConstraints constraints);
}