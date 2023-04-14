namespace GraphManipulation.Models.Interfaces;

public interface IModel<T>
{
    public T Identifier { get; set; }
    public string Description { get; set; }
}