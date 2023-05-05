namespace GraphManipulation.Logging;

public interface ILogger
{
    public void Append(IMutableLog mutableLog);
    public IEnumerable<ILog> Read(ILogConstraints constraints);
}