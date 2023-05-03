using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace IntegrationTests.SystemTest.Tools;

public static class SystemTest
{
    public static string ConfigPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public static string ExecutablePath { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe") : Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation");
    public static string DatabasePath { get; set; } = "system_test_db.sqlite";

    public static void CreateConfigFile(string path)
    {
        CreateConfigFile(path, DatabasePath);
    }
    
    public static void CreateConfigFile(string path, string dbPath)
    {
        var configValues = new Dictionary<string, string>
        {
            {"GraphStoragePath", "test"},
            {"BaseURI", "http://www.test.com/"},
            {"OntologyPath", "test"},
            {"LogPath", "system_test_log.txt"},
            {"DatabaseConnectionString", $"Data Source={dbPath}"},
            {"IndividualsTable", "test"}
        };
        if (!File.Exists(path))
        {
            File.WriteAllText(path, JsonConvert.SerializeObject( configValues ));
        }
    }

    public static TestProcess CreateTestProcess([CallerMemberName] string callerName = "config.json", [CallerFilePath] string fileName = "")
    {
        string configPath = fileName + callerName;
        // If the given argument is a file path, it is assumed to be the path of a config file
        // Otherwise it is the name of the calling method, and a config file should be created for it
        if (!File.Exists(callerName))
        {
            configPath = Path.Combine(Directory.GetCurrentDirectory(), fileName + callerName + ".json");
            string dbPath = fileName + callerName + ".sqlite";
            DeleteDatabase(dbPath);
            CreateConfigFile(configPath, dbPath);
        }

        // Delete database to avoid sharing data across tests


        TestProcess process = new TestProcess(ExecutablePath, configPath);
        return process;
    }
    
    private static void DeleteDatabase(string dbPath)
    {
        File.Delete(dbPath);
    }
}