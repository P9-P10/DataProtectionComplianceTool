using GraphManipulation.Managers;

namespace GraphManipulation.Factories;

public interface IConfigManagerFactory
{
    public IConfigManager CreateConfigManager();
}