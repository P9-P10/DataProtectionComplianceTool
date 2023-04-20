namespace GraphManipulation.Models.Interfaces;

public interface IModel<T> 
{
    public T Identifier { get; }
    public string Description { get; }
    public string Name { get; }
}