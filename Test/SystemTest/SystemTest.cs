using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Test.SystemTest;

public static class SystemTest
{
    public static readonly string DefaultConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public static readonly string DefaultExecutablePath = Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe");
    
    public static void CreateConfigFile(string configPath)
    {
        var configValues = new Dictionary<string, string>
        {
            {"GraphStoragePath", "test"},
            {"BaseURI", "http://www.test.com/"},
            {"OntologyPath", "test"},
            {"LogPath", "system_test_log.txt"},
            {"DatabaseConnectionString", "system_test_db.sqlite"},
            {"IndividualsTable", "test"}
        };
        File.WriteAllText(configPath, JsonConvert.SerializeObject( configValues ));
    }

    public static void CreateConfigFile()
    {
        CreateConfigFile(DefaultConfigPath);
    }
    
    public static TestProcess CreateTestProcess(string executablePath)
    {
        CreateConfigFile();

        TestProcess process = new TestProcess(executablePath);
        return process;
    }

    public static TestProcess CreateTestProcess()
    {
        return CreateTestProcess(DefaultExecutablePath);
    }
}