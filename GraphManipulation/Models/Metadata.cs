using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Metadata<T> : IModel<T>
{
    public T Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
}