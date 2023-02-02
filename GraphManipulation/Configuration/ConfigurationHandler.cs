using System.Configuration;

namespace GraphManipulation.Configuration;

public static class ConfigurationHandler
{
    private const string OntologyKey = "OntologyPath";
    private const string GraphStorageKey = "GraphStorageConnectionString";

    public static string GetConnectionString(Uri databaseUri)
    {
        return ConfigurationManager.ConnectionStrings[databaseUri.ToString()].ConnectionString;
    }

    public static string? GetOntologyPath()
    {
        return ConfigurationManager.AppSettings[OntologyKey];
    }

    // Heavily inspired by
    // https://learn.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager?view=dotnet-plat-ext-7.0
    public static void UpdateOntologyPath(string value)
    {
        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var settings = configFile.AppSettings.Settings;
        if (settings[OntologyKey] is null)
        {
            settings.Add(OntologyKey, value);
        }
        else
        {
            settings[OntologyKey].Value = value;
        }

        configFile.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
    }

    public static string? GetGraphStorageConnectionString()
    {
        try
        {
            return ConfigurationManager.ConnectionStrings[GraphStorageKey].ConnectionString;
        }
        catch (Exception)
        {
            return null;
        }
    }

    // Heavily inspired by
    // https://learn.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager?view=dotnet-plat-ext-7.0
    public static void UpdateGraphStorageConnectionString(string value)
    {
        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var settings = configFile.ConnectionStrings.ConnectionStrings;
        if (settings[GraphStorageKey] is null)
        {
            settings.Add(new ConnectionStringSettings(GraphStorageKey, value));
        }
        else
        {
            settings[GraphStorageKey].ConnectionString = value;
        }

        configFile.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
    }
}