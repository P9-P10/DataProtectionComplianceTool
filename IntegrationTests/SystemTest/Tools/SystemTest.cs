using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace IntegrationTests.SystemTest.Tools;

public static class SystemTest
{
    public static string ConfigPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public static string ExecutablePath { get; set; } = 
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
        ? Path.Combine(Directory.GetCurrentDirectory(), "DataProtectionComplianceTool.exe") 
        : Path.Combine(Directory.GetCurrentDirectory(), "DataProtectionComplianceTool");


    public static void CreateConfigFile(string path, string dbPath, string logPath)
    {
        var configValues = new Dictionary<string, string>
        {
            {"GraphStoragePath", "test"},
            {"BaseURI", "http://www.test.com/"},
            {"OntologyPath", "test"},
            {"LogPath", $"{logPath}"},
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
        var dbPath = SetupDatabase(callerName, fileName);
        var logPath = SetupLog(callerName, fileName);
        var configPath = SetupConfig(callerName, fileName, dbPath, logPath);

        return new TestProcess(ExecutablePath, configPath);
    }

    public static TestProcess CreateTestProcess(out IDbConnection dbConnection,
        [CallerMemberName] string callerName = "", [CallerFilePath] string fileName = "")
    {
        var dbPath = SetupDatabase(callerName, fileName);
        var logPath = SetupLog(callerName, fileName);
        var configPath = SetupConfig(callerName, fileName, dbPath, logPath);

        dbConnection = new SqliteConnection($"Data Source={dbPath}");

        return new TestProcess(ExecutablePath, configPath);
    }

    private static string SetupDatabase(string callerName, string fileName)
    {
        var dbPath = CreateDatabasePath(callerName, fileName);
        DeleteDatabase(dbPath);
        return dbPath;
    }

    private static string SetupLog(string callerName, string fileName)
    {
        var logPath = CreateLogPath(callerName, fileName);
        DeleteLog(logPath);
        return logPath;
    }

    private static string SetupConfig(string callerName, string fileName, string dbPath, string logPath)
    {
        var configPath = CreateConfigPath(callerName, fileName);
        CreateConfigFile(configPath, dbPath, logPath);
        return configPath;
    }

    private static string CreateLogPath(string callerName, string fileName)
    {
        return CreateFileWithExtension(callerName, fileName, ".txt");
    }
    
    private static string CreateConfigPath(string callerName, string fileName)
    {
        return CreateFileWithExtension(callerName, fileName, ".json");
    }

    private static string CreateDatabasePath(string callerName, string fileName)
    {
        return CreateFileWithExtension(callerName, fileName, ".sqlite");
    }
    
    private static string CreateFileWithExtension(string callerName, string fileName, string extension)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileName) + callerName + extension);
    }
    
    private static void DeleteDatabase(string dbPath)
    {
        File.Delete(dbPath);
    }

    private static void DeleteLog(string logPath)
    {
        File.Delete(logPath);
    }
}