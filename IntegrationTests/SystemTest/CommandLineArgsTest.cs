using System.Runtime.InteropServices;
using FluentAssertions;
using IntegrationTests.SystemTest.Tools;
using Newtonsoft.Json;

namespace IntegrationTests.SystemTest;

public class CommandLineArgsTest
{
    private string executablePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
        Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe") : 
        Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation");
    
    [Fact]
    public void NoArgumentsUsesDefaultConfigFile()
    {
        using TestProcess process = new TestProcess(executablePath);
        process.Start();
        process.GiveInput("");
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        result.Should().Be($"Using config found at {IntegrationTests.SystemTest.Tools.SystemTest.ConfigPath}$: ");
    }

    [Fact]
    public void UsesConfigAtGivenPath()
    {
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "testConfig.json"); 
        var configValues = new Dictionary<string, string>
        {
            {"GraphStoragePath", "test"},
            {"BaseURI", "http://www.test.com/"},
            {"OntologyPath", "test"},
            {"LogPath", "system_test_log.txt"},
            {$"DatabaseConnectionString", $"Data Source=TestDb.sqlite"},
            {"IndividualsTable", "test"}
        };

        File.WriteAllText(configPath, JsonConvert.SerializeObject( configValues ));

        using TestProcess process = new TestProcess(executablePath, configPath);
        process.Start();
        process.GiveInput("");
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        result.Should().Be($"Using config found at {configPath}$: ");
    }

    [Fact]
    public void CreatesFileGivenNonExistentFilePath()
    {
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "createdConfig.json");
        File.Delete(configPath);
        File.Exists(configPath).Should().BeFalse();
        
        using TestProcess process = new TestProcess(executablePath, configPath);
        process.Start();
        process.GiveInput("");
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        result.Should().Be($"Please fill GraphStoragePath, OntologyPath, LogPath, "+
                           $"DatabaseConnectionString, IndividualsTable in config file located at: {configPath}");
    }

    [Fact]
    public void PrintsErrorMessageGivenTooManyArguments()
    {
        using TestProcess process = new TestProcess(executablePath, @"too"" many arguments");
        process.Start();
        process.GiveInput("");
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        result.Should().Be("Received too many arguments. Only a single argument specifying the path of the configuration file expected");
    }
    
    [Fact]
    public void PrintsErrorMessageGivenInvalidFilePath()
    {
        using TestProcess process = new TestProcess(executablePath, "Not@File||");
        process.Start();
        process.GiveInput("");
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        string error = string.Join("", process.GetLastError());
        result.Should().Be("The given argument is not a valid filepath");
        error.Should().BeEmpty();
    }

    [Fact]
    public void WrapsGivenFilePathInQuotes()
    {
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "created Config.json");
        File.Delete(configPath);
        File.Exists(configPath).Should().BeFalse();
        
        using TestProcess process = new TestProcess(executablePath, configPath);
        process.ConfigPath.Should().Be(configPath);
        process.Start();
        process.GiveInput("");
        string result = string.Join("", process.GetAllOutputNoWhitespace());
        result.Should().Be($"Please fill GraphStoragePath, OntologyPath, LogPath, "+
                           $"DatabaseConnectionString, IndividualsTable in config file located at: {configPath}");
    }
}