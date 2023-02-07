using Newtonsoft.Json;

namespace GraphManipulation.Helpers;

public class ConfigManager
{
    private string filepath;
    private Dictionary<string, string> config;

    public ConfigManager(String filepath)
    {
        this.filepath = filepath;
        Init();
    }

    public string getValue(string query)
    {
        string result = config[query];
        if (result == "")
        {
            throw new Exception($"Please fill out the config file. {filepath}");
        }

        return result;
    }

    private void Init()
    {
        if (!File.Exists(filepath))
        {
            Dictionary<string, string> initialConfig = new Dictionary<string, string>()
            {
                {"GraphStoragePath", ""},
                {"OptimizedDatabaseName", ""},
                {"SimpleDatabaseName", ""},
                {"DatabasePath", ""},
                {"BaseURI", ""},
                {"OutputFileName", ""},
                {"OutputPath", ""},
                {"OntologyPath", ""}
            };
            Console.WriteLine("Got here atleast.");
            using var file = File.CreateText(filepath);
            var json = JsonConvert.SerializeObject(initialConfig, Formatting.Indented);
            file.Write(json);
        }

        config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filepath)) ??
                 throw new Exception($"Config file in {filepath} is empty, please delete and run the program again.");
    }
}