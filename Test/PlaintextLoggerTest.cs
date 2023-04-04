using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using Xunit;

namespace Test;

public abstract class LogTest : IDisposable
{
    protected LogTest()
    {
        DeleteTestLog();
    }

    public void Dispose()
    {
        DeleteTestLog();
    }

    protected static string GetTestProjectFolder()
    {
        return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
    }

    protected static string GetTestConfigPath()
    {
        return Path.Combine(GetTestProjectFolder(), $"TestResources{Path.DirectorySeparatorChar}testConfig.json");
    }

    protected static string GetTestLogFileName()
    {
        return $"TestResources{Path.DirectorySeparatorChar}log";
    }

    protected static string GetTestLogFilePath()
    {
        return Path.Combine(GetTestProjectFolder(), GetTestLogFileName());
    }

    private static void DeleteTestLog()
    {
        if (File.Exists(GetTestLogFilePath()))
        {
            File.Delete(GetTestLogFilePath());
        }
    }

    protected static ConfigManager CreateConfigManager()
    {
        var configManager = new ConfigManager(GetTestConfigPath());
        configManager.UpdateValue("LogPath", GetTestLogFilePath());
        return configManager;
    }
}

[Collection("Sequential")]
public class PlaintextLoggerTest : LogTest
{
    [Fact]
    public void LoggerThrowsExceptionIfFilePathToLogNotAvailable()
    {
        var configManager = CreateConfigManager();
        configManager.UpdateValue("LogPath", "");

        Assert.Throws<LoggerException>(() => new PlaintextLogger(configManager));
    }

    [Collection("Sequential")]
    public class Append : LogTest
    {
        [Fact]
        public void LoggerCreatesFileIfNotExists()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log = new MutableLog(LogType.SchemaChange, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            Assert.True(File.Exists(GetTestLogFilePath()));
        }

        [Fact]
        public void LoggerWritesToFile()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            var actual = File.ReadLines(GetTestLogFilePath()).First();

            Assert.Contains(
                "Metadata" + Log.LogDelimiter() + "Plaintext" + Log.LogDelimiter() + "Test message",
                actual);
        }

        [Fact]
        public void LoggerAppendsToFile()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");

            logger.Append(log1);
            logger.Append(log2);

            var actual = File.ReadLines(GetTestLogFilePath()).ToList();

            Assert.Equal(2, actual.Count);
            Assert.Contains("Test message 1", actual[0]);
            Assert.Contains("Test message 2", actual[1]);
        }

        [Fact]
        public void LoggerUsesCorrectFormat()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            var actual = File.ReadLines(GetTestLogFilePath()).First();

            Assert.True(Log.IsValidLogString(actual));
        }

        [Fact]
        public void LogEntriesAreNumberedSequentially()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new MutableLog(LogType.SchemaChange, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var actual = File.ReadLines(GetTestLogFilePath()).ToList();

            var index = 1;

            actual.ForEach(logString =>
            {
                Assert.Equal($"{index}", logString.Split(Log.LogDelimiter()).First());
                Assert.Equal($"Test message {index}", logString.Split(Log.LogDelimiter()).Last());
                index++;
            });
        }

        [Fact]
        public void LogFileWithNoLogsStartsAtOne()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            var actual = File.ReadLines(GetTestLogFilePath()).First();

            Assert.Equal("1", actual.Split(Log.LogDelimiter()).First());
        }

        [Fact]
        public void LogFileWithExistingLogsStartsAtLastNumber()
        {
            var configManager = CreateConfigManager();
            var logger1 = new PlaintextLogger(configManager);
            var log1 = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message A");

            logger1.Append(log1);

            var logString = File.ReadAllLines(GetTestLogFilePath()).First();
            var splitString = logString.Split(Log.LogDelimiter());
            splitString[0] = "4567";
            var modifiedLogString = string.Join(Log.LogDelimiter(), splitString);

            File.WriteAllLines(GetTestLogFilePath(), new List<string> { modifiedLogString });

            var logger2 = new PlaintextLogger(configManager);
            var log2 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message B");

            logger2.Append(log2);

            var actual = File.ReadLines(GetTestLogFilePath()).Last();

            Assert.Equal("4568", actual.Split(Log.LogDelimiter()).First());
        }

        [Fact]
        public void LogFileWithLineNumberThatDoesNotParseThrowsException()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");
            logger.Append(log);

            var logString = File.ReadAllText(GetTestLogFilePath());
            var splitString = logString.Split(Log.LogDelimiter());
            splitString[0] = "This should not parse";
            var modifiedLogString = string.Join(Log.LogDelimiter(), splitString);

            File.WriteAllLines(GetTestLogFilePath(), new[] { modifiedLogString });

            Assert.Throws<LoggerException>(() => new PlaintextLogger(configManager));
        }
    }

    [Collection("Sequential")]
    public class Read : LogTest
    {
        [Fact]
        public void ReadReturnsAllLogsWhenGivenEmptyConstraints()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new MutableLog(LogType.SchemaChange, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var logs = logger.Read(new LogConstraints());

            Assert.Equal(3, logs.Count());
        }

        [Fact]
        public void ReadReturnsLogsWithinNumbersRange()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log1 = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var probe1 = logger.Read(new LogConstraints(new NumberRange(1, 1))).ToList();
            var probe2 = logger.Read(new LogConstraints(new NumberRange(2, 2))).ToList();
            var probe3 = logger.Read(new LogConstraints(new NumberRange(2, 3))).ToList();
            var probe4 = logger.Read(new LogConstraints(new NumberRange(4, 4))).ToList();


            Assert.Single(probe1);
            Assert.Equal(1, probe1.First().LogNumber);

            Assert.Single(probe2);
            Assert.Equal(2, probe2.First().LogNumber);

            Assert.Equal(2, probe3.Count);
            Assert.Equal(2, probe3.First().LogNumber);
            Assert.Equal(3, probe3.Skip(1).First().LogNumber);

            Assert.Empty(probe4);
        }

        private static string CreateLogStringWithNumberAndTime(int number, string timeString)
        {
            return $"{number}" + Log.LogDelimiter() +
                   timeString + Log.LogDelimiter() +
                   LogType.Vacuuming + Log.LogDelimiter() +
                   LogMessageFormat.Plaintext + Log.LogDelimiter() +
                   "This is a message";
        }

        [Fact]
        public void ReadReturnsLogsWithinTimeRange()
        {
            var log1String = CreateLogStringWithNumberAndTime(1, "01-01-0001 01:01:01");
            var log2String = CreateLogStringWithNumberAndTime(2, "02-02-0002 02:02:02");
            var log3String = CreateLogStringWithNumberAndTime(3, "03-03-0003 03:03:03");

            File.WriteAllLines(GetTestLogFilePath(), new[] { log1String, log2String, log3String });

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var probe1 = logger.Read(new LogConstraints(
                timeRange: new TimeRange(
                    new DateTime(1, 1, 1, 1, 1, 1),
                    new DateTime(1, 1, 1, 1, 1, 1)))).ToList();
            var probe2 = logger.Read(new LogConstraints(
                timeRange: new TimeRange(
                    new DateTime(1, 2, 2),
                    new DateTime(3, 2, 2)))).ToList();

            var probe3 = logger.Read(new LogConstraints(
                timeRange: new TimeRange(
                    new DateTime(2, 2, 2, 2, 2, 2),
                    new DateTime(3, 3, 3, 3, 3, 3)))).ToList();

            var probe4 = logger.Read(new LogConstraints(
                timeRange: new TimeRange(
                    new DateTime(4, 4, 4),
                    new DateTime(4, 4, 4)))).ToList();

            Assert.Single(probe1);
            Assert.Equal(1, probe1.First().LogNumber);

            Assert.Single(probe2);
            Assert.Equal(2, probe2.First().LogNumber);

            Assert.Equal(2, probe3.Count);
            Assert.Equal(2, probe3.First().LogNumber);
            Assert.Equal(3, probe3.Skip(1).First().LogNumber);

            Assert.Empty(probe4);
        }

        [Fact]
        public void ReadReturnsLogsWithGivenLogTypes()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log1 = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var probe1 = logger.Read(new LogConstraints(logTypes: new List<LogType>
            {
                LogType.Metadata
            })).ToList();
            var probe2 = logger.Read(new LogConstraints(logTypes: new List<LogType>
            {
                LogType.Vacuuming
            })).ToList();
            var probe3 = logger.Read(new LogConstraints(logTypes: new List<LogType>
            {
                LogType.Metadata, LogType.Vacuuming
            })).ToList();
            var probe4 = logger.Read(new LogConstraints(logTypes: new List<LogType>
                {
                    LogType.SchemaChange
                }))
                .ToList();

            Assert.Single(probe1);
            Assert.Equal(1, probe1.First().LogNumber);

            Assert.Equal(2, probe2.Count);
            Assert.Equal(2, probe2.First().LogNumber);
            Assert.Equal(3, probe2.Skip(1).First().LogNumber);

            Assert.Equal(3, probe3.Count);
            Assert.Equal(1, probe3.First().LogNumber);
            Assert.Equal(2, probe3.Skip(1).First().LogNumber);
            Assert.Equal(3, probe3.Skip(2).First().LogNumber);

            Assert.Empty(probe4);
        }

        [Fact]
        public void ReadReturnsLogsWithGivenLogMessageFormats()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log1 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "");
            var log2 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Json, "");
            var log3 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Json, "");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var probe1 = logger.Read(new LogConstraints(logMessageFormats: new List<LogMessageFormat>
            {
                LogMessageFormat.Plaintext
            })).ToList();
            var probe2 = logger.Read(new LogConstraints(logMessageFormats: new List<LogMessageFormat>
            {
                LogMessageFormat.Json
            })).ToList();
            var probe3 = logger.Read(new LogConstraints(logMessageFormats: new List<LogMessageFormat>
            {
                LogMessageFormat.Plaintext, LogMessageFormat.Json
            })).ToList();
            var probe4 = logger.Read(new LogConstraints(logMessageFormats: new List<LogMessageFormat>
            {
                LogMessageFormat.Turtle
            })).ToList();

            Assert.Single(probe1);
            Assert.Equal(1, probe1.First().LogNumber);

            Assert.Equal(2, probe2.Count);
            Assert.Equal(2, probe2.First().LogNumber);
            Assert.Equal(3, probe2.Skip(1).First().LogNumber);

            Assert.Equal(3, probe3.Count);
            Assert.Equal(1, probe3.First().LogNumber);
            Assert.Equal(2, probe3.Skip(1).First().LogNumber);
            Assert.Equal(3, probe3.Skip(2).First().LogNumber);

            Assert.Empty(probe4);
        }

        [Fact]
        public void ReadReturnsOnlyLogThatMatchesAllOptions()
        {
            // Assert.True(false);

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log1 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "");
            var log2 = new MutableLog(LogType.SchemaChange, LogMessageFormat.Json, "");
            var log3 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Turtle, "");
            var log4 = new MutableLog(LogType.Metadata, LogMessageFormat.Plaintext, "");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);
            logger.Append(log4);

            var logStrings = File.ReadLines(GetTestLogFilePath());

            var timeIndex = 1;

            var modifiedStrings = logStrings.Select(s =>
            {
                var split = s.Split(Log.LogDelimiter());
                split[1] = $"0{timeIndex}-0{timeIndex}-000{timeIndex} 0{timeIndex}:0{timeIndex}:0{timeIndex}";
                timeIndex++;
                return string.Join(Log.LogDelimiter(), split);
            }).ToList();

            File.WriteAllLines(GetTestLogFilePath(), modifiedStrings);

            var probe = logger.Read(new LogConstraints(
                    timeRange: new TimeRange(new DateTime(1, 1, 1, 1, 1, 1), new DateTime(1, 1, 1, 1, 1, 1)),
                    logTypes: new List<LogType> { LogType.Vacuuming },
                    logMessageFormats: new List<LogMessageFormat> { LogMessageFormat.Plaintext }
                )
            );

            Assert.Single(probe);
        }

        [Fact]
        public void ReadReturnsLogsSortedByLogNumber()
        {
            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log1 = new MutableLog(LogType.Vacuuming, LogMessageFormat.Plaintext, "");
            var log2 = new MutableLog(LogType.SchemaChange, LogMessageFormat.Json, "");

            logger.Append(log2);
            logger.Append(log1);

            var probe = logger.Read(new LogConstraints()).ToList();

            Assert.Equal(1, probe.First().LogNumber);
            Assert.Equal(2, probe.Skip(1).First().LogNumber);
        }
    }
}