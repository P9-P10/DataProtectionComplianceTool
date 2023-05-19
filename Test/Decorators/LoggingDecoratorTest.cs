using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GraphManipulation.Decorators;
using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Utility;
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
            return Log.Select(log => new Log(1, DateTime.MinValue, log));
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

        Assert.Equal(FeedbackEmitterMessage.ResultMessage<string, TestEntity>("key", SystemOperation.Operation.Deleted, null, null), message);
    }

    [Fact]
    public void LogUpdateCreatesExpectedMessageWithNullParameters()
    {
        _decorator.LogUpdate("key", null);

        var message = GetMessage();

        Assert.Equal(FeedbackEmitterMessage.ResultMessage<string, TestEntity>("key", SystemOperation.Operation.Updated, null, null), message);
    }

    [Fact]
    public void LogUpdateCreatesExpectedMessageWithParameters()
    {
        var value = new TestEntity { Key = "key", Description = "Description" };
        _decorator.LogUpdate("key", value);

        var message = GetMessage();
        
        Assert.Equal(FeedbackEmitterMessage.ResultMessage<string, TestEntity>("key", SystemOperation.Operation.Updated, null, value), message);
    }

    [Fact]
    public void LogCreateCreatesExpectedMessage()
    {
        _decorator.LogCreate("key");

        var message = GetMessage();

        Assert.Equal(FeedbackEmitterMessage.ResultMessage<string, TestEntity>("key", SystemOperation.Operation.Created, null, null), message);
    }
}