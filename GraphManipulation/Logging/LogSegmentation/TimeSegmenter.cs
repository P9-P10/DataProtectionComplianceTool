using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging.LogSegmentation;

public class TimeSegmenter : ILogFileSegmenter
{
    public bool IsCriteriaMetForSegmentation(ILog log)
    {
        throw new NotImplementedException();
    }

    public string GetLogFileSuffixFromLastLog(ILog log)
    {
        throw new NotImplementedException();
    }
}