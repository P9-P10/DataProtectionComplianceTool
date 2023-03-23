using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Logging.Logs;
using Xunit;

namespace Test;

[Collection("Sequential")]
public class PlaintextLoggerTest
{
    private static string GetTestProjectFolder() =>
        Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

    private static string GetTestConfigPath()
    {
        var projectFolder = GetTestProjectFolder();
        return Path.Combine(projectFolder, "TestResources/config.json");
    }

    private static string GetTestLogPath()
    {
        var projectFolder = GetTestProjectFolder();
        return Path.Combine(projectFolder, "TestResources/log");
    }

    private static void DeleteLog()
    {
        if (File.Exists(GetTestLogPath()))
        {
            File.Delete(GetTestLogPath());
        }
    }

    private static ConfigManager CreateConfigManager()
    {
        var configManager = new ConfigManager(GetTestConfigPath());
        configManager.UpdateValue("LogPath", GetTestLogPath());
        return configManager;
    }

    [Fact]
    public void LoggerThrowsExceptionIfFilePathToLogNotAvailable()
    {
        var configManager = CreateConfigManager();
        configManager.UpdateValue("LogPath", "");

        Assert.Throws<LoggerException>(() => new PlaintextLogger(configManager));
    }

    [Collection("Sequential")]
    public class Append
    {
        [Fact]
        public void LoggerCreatesFileIfNotExists()
        {
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log = new Log(LogType.SchemaChange, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            Assert.True(File.Exists(GetTestLogPath()));
        }

        [Fact]
        public void LoggerWritesToFile()
        {
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            var actual = File.ReadLines(GetTestLogPath()).First();

            Assert.Contains("Metadata" + Log.LogDelimiter() + "Plaintext" + Log.LogDelimiter() + "Test message",
                actual);
        }

        [Fact]
        public void LoggerAppendsToFile()
        {
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");

            logger.Append(log1);
            logger.Append(log2);

            var actual = File.ReadLines(GetTestLogPath()).ToList();

            Assert.Equal(2, actual.Count);
            Assert.Contains("Test message 1", actual[0]);
            Assert.Contains("Test message 2", actual[1]);
        }

        [Fact]
        public void LoggerUsesCorrectFormat()
        {
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var log = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            var actual = File.ReadLines(GetTestLogPath()).First();

            Assert.Equal("1" + Log.LogDelimiter() +
                         log.GetCreationTimeStamp() + Log.LogDelimiter() +
                         "Metadata" + Log.LogDelimiter() +
                         "Plaintext" + Log.LogDelimiter() +
                         "Test message",
                actual);
            Assert.True(Log.IsValidLogString(actual));
        }

        [Fact]
        public void LogEntriesAreNumberedSequentially()
        {
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var actual = File.ReadLines(GetTestLogPath()).ToList();

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
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");

            logger.Append(log);

            var actual = File.ReadLines(GetTestLogPath()).First();

            Assert.Equal("1", actual.Split(Log.LogDelimiter()).First());
        }

        [Fact]
        public void LogFileWithExistingLogsStartsAtLastNumber()
        {
            DeleteLog();

            var log1 = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message A");
            var log2 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message B");

            File.WriteAllLines(GetTestLogPath(), new[] { "4567" + Log.LogDelimiter() + log1.LogToString() });

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            logger.Append(log2);

            var actual = File.ReadLines(GetTestLogPath()).Last();

            Assert.Equal("4568", actual.Split(Log.LogDelimiter()).First());
        }

        [Fact]
        public void LogFileWithLineNumberThatDoesNotParseThrowsException()
        {
            DeleteLog();

            var log = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message");
            File.WriteAllLines(GetTestLogPath(),
                new[] { "This should not pass" + Log.LogDelimiter() + log.LogToString() });

            var configManager = CreateConfigManager();
            Assert.Throws<LoggerException>(() => new PlaintextLogger(configManager));
        }
    }

    [Collection("Sequential")]
    public class Read
    {
        [Fact]
        public void ReadReturnsAllLogsWhenGivenEmptyOptions()
        {
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new Log(LogType.SchemaChange, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var logs = logger.Read(new LoggerReadOptions());

            Assert.Equal(3, logs.Count());
        }

        [Fact]
        public void ReadReturnsLogsWithinNumbersRange()
        {
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var probe1 = logger.Read(new LoggerReadOptions(logNumberRange: new NumberRange(1, 1))).ToList();
            var probe2 = logger.Read(new LoggerReadOptions(logNumberRange: new NumberRange(2, 2))).ToList();
            var probe3 = logger.Read(new LoggerReadOptions(logNumberRange: new NumberRange(2, 3))).ToList();
            var probe4 = logger.Read(new LoggerReadOptions(logNumberRange: new NumberRange(4, 4))).ToList();


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
            DeleteLog();

            var log1String = CreateLogStringWithNumberAndTime(1, "01/01/0001 01.01.01");
            var log2String = CreateLogStringWithNumberAndTime(2, "02/02/0002 02.02.02");
            var log3String = CreateLogStringWithNumberAndTime(3, "03/03/0003 03.03.03");

            File.WriteAllLines(GetTestLogPath(), new[] { log1String, log2String, log3String });

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);

            var probe1 = logger.Read(new LoggerReadOptions(
                timeRange: new TimeRange(
                    new DateTime(1, 1, 1, 1, 1, 1),
                    new DateTime(1, 1, 1, 1, 1, 1)))).ToList();
            var probe2 = logger.Read(new LoggerReadOptions(
                timeRange: new TimeRange(
                    new DateTime(1, 2, 2),
                    new DateTime(3, 2, 2)))).ToList();

            var probe3 = logger.Read(new LoggerReadOptions(
                timeRange: new TimeRange(
                    new DateTime(2, 2, 2, 2, 2, 2),
                    new DateTime(3, 3, 3, 3, 3, 3)))).ToList();

            var probe4 = logger.Read(new LoggerReadOptions(
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
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new Log(LogType.Metadata, LogMessageFormat.Plaintext, "Test message 1");
            var log2 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 2");
            var log3 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "Test message 3");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var probe1 = logger.Read(new LoggerReadOptions(logTypes: new List<LogType>
                {
                    LogType.Metadata
                })).ToList();
            var probe2 = logger.Read(new LoggerReadOptions(logTypes: new List<LogType>
                {
                    LogType.Vacuuming
                })).ToList();
            var probe3 = logger.Read(new LoggerReadOptions(logTypes: new List<LogType>
                {
                    LogType.Metadata, LogType.Vacuuming
                })).ToList();
            var probe4 = logger.Read(new LoggerReadOptions(logTypes: new List<LogType>
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
            DeleteLog();

            var configManager = CreateConfigManager();
            var logger = new PlaintextLogger(configManager);
            var log1 = new Log(LogType.Vacuuming, LogMessageFormat.Plaintext, "");
            var log2 = new Log(LogType.Vacuuming, LogMessageFormat.Json, "");
            var log3 = new Log(LogType.Vacuuming, LogMessageFormat.Json, "");

            logger.Append(log1);
            logger.Append(log2);
            logger.Append(log3);

            var probe1 = logger.Read(new LoggerReadOptions(logMessageFormats: new List<LogMessageFormat>
            {
                LogMessageFormat.Plaintext
            })).ToList();
            var probe2 = logger.Read(new LoggerReadOptions(logMessageFormats: new List<LogMessageFormat>
            {
                LogMessageFormat.Json
            })).ToList();
            var probe3 = logger.Read(new LoggerReadOptions(logMessageFormats: new List<LogMessageFormat>
            {
                LogMessageFormat.Plaintext, LogMessageFormat.Json
            })).ToList();
            var probe4 = logger.Read(new LoggerReadOptions(logMessageFormats: new List<LogMessageFormat>
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
    }
}