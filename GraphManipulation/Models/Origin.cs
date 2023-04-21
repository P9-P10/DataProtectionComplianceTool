using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Origin<T> : IModel<T>
{
    public T Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; }
}