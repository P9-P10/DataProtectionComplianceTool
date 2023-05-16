using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Factories.Interfaces;

public interface IManagerFactory
{
    public IManager<TK, TV> CreateManager<TK, TV>() where TV : Entity<TK>, new() where TK : notnull;
}
