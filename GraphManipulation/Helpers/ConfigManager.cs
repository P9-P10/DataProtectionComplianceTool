using Newtonsoft.Json;

namespace GraphManipulation.Helpers;

public class ConfigManager
{
    private readonly string _filepath;
    private Dictionary<string, string> _config;

    public ConfigManager(string filepath)
    {
        _config = new Dictionary<string, string>();
        _filepath = filepath;
        Init();
    }

    public string GetValue(string query)
    {
        if (!_config.ContainsKey(query))
        {
            throw new KeyNotFoundException();
        }

        var result = _config[query];
        return result;
    }

    public void UpdateValue(string key, string value)
    {
        _config[key] = value;
        var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
        File.WriteAllText(_filepath, json);
    }

    private void Init()
    {
        if (!File.Exists(_filepath))
        {
            var initialConfig = new Dictionary<string, string>
            {
                { "GraphStoragePath", "" },
                { "OptimizedDatabaseName", "OptimizedAdvancedDatabase.sqlite" },
                { "SimpleDatabaseName", "SimpleDatabase.sqlite" },
                { "DatabasePath", "" },
                { "BaseURI", "http://www.test.com/" },
                { "OutputFileName", "output.ttl" },
                { "OutputPath", "" },
                { "OntologyPath", "" },
                { "LogPath", "" }
            };
            using var file = File.CreateText(_filepath);
            var json = JsonConvert.SerializeObject(initialConfig, Formatting.Indented);
            file.Write(json);
        }

        _config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_filepath)) ??
                  throw new Exception($"Config file in {_filepath} is empty, please delete and run the program again.");
    }
}