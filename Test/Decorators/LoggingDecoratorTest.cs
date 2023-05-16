using System.Collections.Generic;
using GraphManipulation.Decorators;
using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;
using Xunit;

namespace Test.Decorators;

public class LoggingDecoratorTest
{
    private class TestLogger : ILogger
    {
        public List<IMutableLog> Log { get; set; }

        public TestLogger()
        {
            Log = new List<IMutableLog>();
        }

        public void Append(IMutableLog mutableLog)
        {
            Log.Add(mutableLog);
        }

        public IEnumerable<ILog> Read(ILogConstraints constraints)
        {
            throw new System.NotImplementedException();
        }
    }

    private class TestEntity : Entity<string>
    {
    }

    private readonly TestLogger _logger;
    private readonly LoggingDecorator<string, TestEntity> _decorator;

    public LoggingDecoratorTest()
    {
        _logger = new TestLogger();
        _decorator = new LoggingDecorator<string, TestEntity>(_logger);
    }

    private string GetMessage()
    {
        return _logger.Log[0].Message;
    }

    [Fact]
    public void LogDeleteCreatesExpectedMessage()
    {
        _decorator.LogDelete("key");

        var message = GetMessage();

        Assert.Equal("Test entity 'key' deleted.", message);
    }

    [Fact]
    public void LogUpdateCreatesExpectedMessageWithNullParameters()
    {
        _decorator.LogUpdate("key", null);

        var message = GetMessage();

        Assert.Equal("Test entity 'key' updated.", message);
    }

    [Fact]
    public void LogUpdateCreatesExpectedMessageWithParameters()
    {
        _decorator.LogUpdate("key", new TestEntity { Key = "key", Description = "Description" });

        var message = GetMessage();

        Assert.Equal("Test entity 'key' updated. Value: key, Description", message);
    }

    [Fact]
    public void LogCreateCreatesExpectedMessage()
    {
        _decorator.LogCreate("key");

        var message = GetMessage();

        Assert.Equal("Test entity 'key' created.", message);
    }
}