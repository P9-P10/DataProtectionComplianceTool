using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging;

public interface ILogFileSegmenter
{
    public bool IsCriteriaMetForSegmentation(ILog log);
}