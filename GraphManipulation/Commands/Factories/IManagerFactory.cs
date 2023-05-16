using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands.Factories;

public interface IManagerFactory
{
    public IManager<TK, TV> CreateManager<TK, TV>() where TV : Entity<TK>, new() where TK : notnull;
}
