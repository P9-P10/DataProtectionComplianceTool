using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Logging;

namespace GraphManipulation.Factories;

public class PlaintextLoggerFactory : ILoggerFactory
{
    private readonly IConfigManagerFactory? _configManagerFactory;
    private readonly ILogger? _logger;

    public PlaintextLoggerFactory(IConfigManagerFactory configManagerFactory)
    {
        _configManagerFactory = configManagerFactory;
    }

    public PlaintextLoggerFactory(ILogger logger)
    {
        _logger = logger;
    }

    public ILogger CreateLogger()
    {
        return _logger ?? new PlaintextLogger(_configManagerFactory.CreateConfigManager());
    }
}