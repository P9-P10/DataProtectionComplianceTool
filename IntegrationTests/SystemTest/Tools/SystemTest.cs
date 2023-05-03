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

    public static TestProcess CreateTestProcess([CallerMemberName] string callerName = "", [CallerFilePath] string fileName = "")
    {
        
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName) + callerName + ".json");
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName) + callerName + ".sqlite");
        DeleteDatabase(dbPath);
        CreateConfigFile(configPath, dbPath);
        
        TestProcess process = new TestProcess(ExecutablePath, configPath);
        return process;
    }
    
    private static void DeleteDatabase(string dbPath)
    {
        File.Delete(dbPath);
    }
}