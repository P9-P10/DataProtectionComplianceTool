using System;
using GraphManipulation.Logging.Logs;
using Xunit;

namespace Test;

public class LogTest
{

    [Fact]
    public void LogToStringReturnsCorrectString()
    {
        var log = new Log(47, DateTime.Now, LogType.Vacuuming, LogMessageFormat.Plaintext, "This is a message");
        Assert.Equal("47" + Log.LogDelimiter() + log.GetCreationTimeStamp() + Log.LogDelimiter() + "Vacuuming" + Log.LogDelimiter() + "Plaintext" + Log.LogDelimiter() + "This is a message", log.LogToString());
    }

    [Fact]
    public void LogFromStringInvalidStringThrowsException()
    {
        const string logString = "This should not pass";
        Assert.Throws<LogException>(() => new Log(logString));
    }
    
    private static string GetTestLogString() => "123" + Log.LogDelimiter() + 
                                                "12/05/1937 09.57.33" + Log.LogDelimiter() +
                                                LogType.Vacuuming + Log.LogDelimiter() +
                                                LogMessageFormat.Plaintext + Log.LogDelimiter() + 
                                                "This is a message";

    [Fact]
    public void LogFromStringGetsCreationTime()
    {
        var logString = GetTestLogString();
        var log = new Log(logString);

        var expectedDateTime = new DateTime(1937, 5, 12, 9, 57, 33);
        Assert.Equal(expectedDateTime, log.CreationTime);
    }

    [Fact]
    public void LogFromStringGetsLogType()
    {
        var logString = GetTestLogString();
        var log = new Log(logString);

        Assert.Equal(LogType.Vacuuming, log.LogType);
    }

    [Fact]
    public void LogFromStringLogTypeInvalidThrowsException()
    {
        var invalidLogString = GetTestLogString().Replace(LogType.Vacuuming.ToString(), "This should not parse");
        Assert.Throws<LogException>(() => new Log(invalidLogString));
    }
    

    [Fact]
    public void LogFromStringGetsLogMessageFormat()
    {
        var logString = GetTestLogString();
        var log = new Log(logString);

        Assert.Equal(LogMessageFormat.Plaintext, log.LogMessageFormat);
    }

    [Fact]
    public void LogFromStringLogMessageFormatInvalidThrowsException()
    {
        var invalidLogString = GetTestLogString().Replace(LogMessageFormat.Plaintext.ToString(), "This should not parse");
        Assert.Throws<LogException>(() => new Log(invalidLogString));
    }

    [Fact]
    public void LogFromStringGetsMessage()
    {
        var logString = GetTestLogString();
        var log = new Log(logString);

        Assert.Equal("This is a message", log.Message);
    }

    [Fact]
    public void LogFromStringGetsLogNumber()
    {
        var logString = GetTestLogString();
        var log = new Log(logString);
        
        Assert.Equal(123, log.LogNumber);
    }

    [Fact]
    public void LogFromStringInvalidLogNumberThrowsException()
    {
        var invalidLogString = GetTestLogString().Replace("123", "This should not parse");
        Assert.Throws<LogException>(() => new Log(invalidLogString));
    }
}