using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Test.SystemTest;

public static class SystemTest
{
    public static readonly string DefaultConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public static readonly string DefaultExecutablePath = Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe");
    public static string DatabasePath { get; set; } = "system_test_db.sqlite";

    public static void CreateConfigFile(string configPath)
    {
        var configValues = new Dictionary<string, string>
        {
            {"GraphStoragePath", "test"},
            {"BaseURI", "http://www.test.com/"},
            {"OntologyPath", "test"},
            {"LogPath", "system_test_log.txt"},
            {$"DatabaseConnectionString", $"Data Source={DatabasePath}"},
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
        // Delete database to avoid sharing data across tests
        DeleteDatabase();

        TestProcess process = new TestProcess(executablePath);
        return process;
    }
    
    private static void DeleteDatabase()
    {
        File.Delete(DatabasePath);
    }

    public static TestProcess CreateTestProcess()
    {
        return CreateTestProcess(DefaultExecutablePath);
    }
}