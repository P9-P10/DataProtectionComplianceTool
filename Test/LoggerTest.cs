using System.IO;
using System.Linq;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Logging.Logs;
using Xunit;

namespace Test;

public class LoggerTest
{
    private static string GetTestProjectFolder() => Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
    
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

        Assert.Contains("Metadata" + Log.LogDelimiter() + "Plaintext" + Log.LogDelimiter() + "Test message", actual);
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
        
        File.WriteAllLines(GetTestLogPath(), new []{"4567" + Log.LogDelimiter() + log1.LogToString()});

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
        File.WriteAllLines(GetTestLogPath(), new []{"This should not pass" + Log.LogDelimiter() + log.LogToString()});

        var configManager = CreateConfigManager();
        Assert.Throws<LoggerException>(() => new PlaintextLogger(configManager));
    }
}