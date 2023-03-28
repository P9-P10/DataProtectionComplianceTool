using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging.LogSegmentation;

public class AlwaysSegmenter : ILogFileSegmenter
{
    public bool IsCriteriaMetForSegmentation(ILog log)
    {
        return true;
    }

    public string GetLogFileSuffixFromLastLog(ILog log)
    {
        return "Always";
    }
}