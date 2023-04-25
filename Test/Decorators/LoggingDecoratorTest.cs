﻿

using System.Collections.Generic;
using System.Linq;
using GraphManipulation.Decorators;
using Xunit;
using GraphManipulation.Logging;

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

        public IOrderedEnumerable<ILog> Read(ILogConstraints constraints)
        {
            throw new System.NotImplementedException();
        }
    }

    private TestLogger logger;
    private LoggingDecorator decorator;

    public LoggingDecoratorTest()
    {
        logger = new TestLogger();
        decorator = new LoggingDecorator(logger, "TestType");
    }

    private string GetMessage()
    {
        return logger.Log[0].Message;
    }
    
    [Fact]
    public void LogDeleteCreatesExpectedMessage()
    {
        decorator.LogDelete("key");

        string message = GetMessage();
        
        Assert.Equal("Delete TestType key.", message);
    }

    [Fact]
    public void LogUpdateCreatesExpectedMessageWithNullParameters()
    {
        decorator.LogUpdate("key", null);

        string message = GetMessage();
        
        Assert.Equal("Update TestType key.", message);
    }

    [Fact]
    public void LogUpdateCreatesExpectedMessageWithParameters()
    {
        decorator.LogUpdate("key", new { Param1 = "one", Param2 = "two" });

        string message = GetMessage();
        
        Assert.Equal("Update TestType key. Parameters: [Param1, one], [Param2, two]", message);
    }

    [Fact]
    public void LogAddCreatesExpectedMessageWithNullParameters()
    {
        decorator.LogAdd("key", null);

        string message = GetMessage();
        
        Assert.Equal("Add TestType key.", message);
    }

    [Fact]
    public void LogAddCreatesExpectedMessageWithParameters()
    {
        decorator.LogAdd("key", new { Param1 = "one", Param2 = "two" });

        string message = GetMessage();
        
        Assert.Equal("Add TestType key. Parameters: [Param1, one], [Param2, two]", message);
    }

}