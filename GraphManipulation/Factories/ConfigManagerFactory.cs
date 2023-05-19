using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;

namespace GraphManipulation.Factories;

public class ConfigManagerFactory : IConfigManagerFactory
{
    private readonly IConfigManager? _configManager;
    private readonly string _filepath;

    public ConfigManagerFactory(string filepath)
    {
        _filepath = filepath;
    }

    public ConfigManagerFactory(IConfigManager configManager)
    {
        _filepath = "";
        _configManager = configManager;
    }

    public IConfigManager CreateConfigManager()
    {
        return _configManager ?? new ConfigManager(_filepath);
    }
}