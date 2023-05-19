namespace GraphManipulation.Logging;

public interface ILogConstraints
{
    IEnumerable<ILog> ApplyConstraintsToLogs(IEnumerable<ILog> logs);
}