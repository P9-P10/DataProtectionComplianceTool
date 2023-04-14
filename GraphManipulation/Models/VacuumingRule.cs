using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class VacuumingRule<T> : IModel<T>
{
    public T Identifier { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string Rule { get; set; }
}