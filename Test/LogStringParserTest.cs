using System;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Logging.Logs;
using Xunit;

namespace Test;

public class LogStringParserTest
{
    [Fact]
    public void AnyParseOnInvalidStringThrowsException()
    {
        const string logString = "This should not pass";
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseLogNumber(logString));
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseCreationTime(logString));
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseLogType(logString));
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseLogMessageFormat(logString));
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseLogMessage(logString));
    }

    private static string GetTestLogString()
    {
        return "123" + Log.LogDelimiter() +
               "12/05/1937 09.57.33" + Log.LogDelimiter() +
               LogType.Vacuuming + Log.LogDelimiter() +
               LogMessageFormat.Plaintext + Log.LogDelimiter() +
               "This is a message";
    }

    [Fact]
    public void ParseCreationTimeReturnsCorrectDateTime()
    {
        var expected = new DateTime(1937, 5, 12, 9, 57, 33);
        var actual = LogStringParser.ParseCreationTime(GetTestLogString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParseCreationTimeInvalidTimeStringThrowsException()
    {
        var invalidLogString = GetTestLogString().Replace("12/05/1937 09.57.33", "This should not parse");
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseCreationTime(invalidLogString));
    }

    [Fact]
    public void ParseLogTypeReturnsCorrectLogType()
    {
        var expected = LogType.Vacuuming;
        var actual = LogStringParser.ParseLogType(GetTestLogString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParseLogTypeInvalidThrowsException()
    {
        var invalidLogString = GetTestLogString().Replace(LogType.Vacuuming.ToString(), "This should not parse");
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseLogType(invalidLogString));
    }


    [Fact]
    public void ParseLogMessageFormatReturnsCorrectLogMessageFormat()
    {
        var expected = LogMessageFormat.Plaintext;
        var actual = LogStringParser.ParseLogMessageFormat(GetTestLogString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParseLogMessageFormatInvalidThrowsException()
    {
        var invalidLogString =
            GetTestLogString().Replace(LogMessageFormat.Plaintext.ToString(), "This should not parse");
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseLogMessageFormat(invalidLogString));
    }

    [Fact]
    public void ParseLogMessageReturnsCorrectMessage()
    {
        var expected = "This is a message";
        var actual = LogStringParser.ParseLogMessage(GetTestLogString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ParseLogNumberReturnsCorrectLogNumber()
    {
        var expected = 123;
        var actual = LogStringParser.ParseLogNumber(GetTestLogString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LogFromStringInvalidLogNumberThrowsException()
    {
        var invalidLogString = GetTestLogString().Replace("123", "This should not parse");
        Assert.Throws<LogStringParserException>(() => LogStringParser.ParseLogNumber(invalidLogString));
    }
}