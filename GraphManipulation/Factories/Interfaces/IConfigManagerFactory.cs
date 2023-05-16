using GraphManipulation.Managers;

namespace GraphManipulation.Factories.Interfaces;

public interface IConfigManagerFactory
{
    public IConfigManager CreateConfigManager();
}