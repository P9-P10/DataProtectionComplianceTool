using GraphManipulation.Helpers;

namespace GraphManipulation.Commands.Factories;

public interface IConfigManagerFactory
{
    public IConfigManager CreateConfigManager();
}