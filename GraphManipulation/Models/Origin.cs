using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Origin<T> : IModel<T>
{
    public T Identifier { get; set; }
    public string Description { get; set; }
}