using Newtonsoft.Json;

namespace GraphManipulation.Managers;

public class ConfigManager : IConfigManager
{
    private readonly string _filepath;
    private Dictionary<string, string> _config;

    public ConfigManager(string filepath)
    {
        _config = new Dictionary<string, string>();
        _filepath = filepath;
        Init();
    }
    
    public ConfigManager(string filepath,Dictionary<string,string> config)
    {
        _config = config;
        _filepath = filepath;
        Init();
    }

    public string GetValue(string query)
    {
        if (!_config.ContainsKey(query))
        {
            throw new KeyNotFoundException(
                $"Please make sure that the config file located at {_filepath} is correctly set up, " +
                $"the key {query} could not be found");
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

    public List<string> GetEmptyKeys()
    {
        return (from configElement in _config where string.IsNullOrEmpty(configElement.Value) select configElement.Key)
            .ToList();
    }

    private void Init()
    {
        if (!File.Exists(_filepath))
        {
            if (_config.Count == 0)
            {
                _config = new Dictionary<string, string>
                {
                    {"GraphStoragePath", ""},
                    {"BaseURI", "http://www.test.com/"},
                    {"OntologyPath", ""},
                    {"LogPath", ""},
                    {"DatabaseConnectionString", ""},
                    {"IndividualsTable", ""}
                };
            }
            using var file = File.CreateText(_filepath);
            var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
            file.Write(json);
        }

        _config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_filepath)) ??
                  throw new Exception($"Config file in {_filepath} is empty, please delete and run the program again.");
    }
}