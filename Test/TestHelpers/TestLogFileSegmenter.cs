using GraphManipulation.Logging;
using GraphManipulation.Logging.Logs;

namespace Test.TestHelpers;

public class TestLogFileSegmenter : ILogFileSegmenter
{
    public bool IsCriteriaMetForSegmentation(ILog log)
    {
        throw new System.NotImplementedException();
    }
}