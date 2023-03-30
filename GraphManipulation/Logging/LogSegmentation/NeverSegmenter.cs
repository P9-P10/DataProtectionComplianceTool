using GraphManipulation.Logging.Logs;

namespace GraphManipulation.Logging.LogSegmentation;

public class NeverSegmenter : ILogFileSegmenter
{
    public bool IsCriteriaMetForSegmentation(ILog log)
    {
        return false;
    }

    public string GetLogFileSuffixFromLastLog(ILog log)
    {
        return "Never";
    }
}