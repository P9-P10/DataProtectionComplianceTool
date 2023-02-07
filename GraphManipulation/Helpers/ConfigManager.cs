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

    public string GetValue(string query)
    {
        if (!config.ContainsKey(query)) throw new KeyNotFoundException();
        string result = config[query];
        return result;

    }

    public void UpdateValue(string key, string value)
    {
        config[key] = value;
        var json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(filepath, json);
    }

    private void Init()
    {
        if (!File.Exists(filepath))
        {
            Dictionary<string, string> initialConfig = new Dictionary<string, string>()
            {
                {"GraphStoragePath", ""},
                {"OptimizedDatabaseName", "OptimizedAdvancedDatabase.sqlite"},
                {"SimpleDatabaseName", "SimpleDatabase.sqlite"},
                {"DatabasePath", ""},
                {"BaseURI", "http://www.test.com/"},
                {"OutputFileName", ""},
                {"OutputPath", "output.ttl"},
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