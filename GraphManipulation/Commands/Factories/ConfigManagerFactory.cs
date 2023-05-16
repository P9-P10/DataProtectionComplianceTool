using GraphManipulation.Helpers;

namespace GraphManipulation.Commands.Factories;

public class ConfigManagerFactory : IConfigManagerFactory
{
    private readonly string _filepath;
    private readonly IConfigManager? _configManager;
    
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