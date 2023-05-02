using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace IntegrationTests.SystemTest.Tools;

public static class SystemTest
{
    public static string ConfigPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public static string ExecutablePath { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe") : Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation");
    public static string DatabasePath { get; set; } = "system_test2_db.sqlite";

    public static void CreateConfigFile()
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
        if (!File.Exists(ConfigPath))
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject( configValues ));
        }
    }

    public static TestProcess CreateTestProcess([CallerMemberName] string caller = null)
    {
        CreateConfigFile();
        // Delete database to avoid sharing data across tests
        DeleteDatabase();
        
        TestProcess process = new TestProcess(ExecutablePath);
        return process;
    }
    
    private static void DeleteDatabase()
    {
        File.Delete(DatabasePath);
    }
}