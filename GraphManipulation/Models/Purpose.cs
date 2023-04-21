using GraphManipulation.Managers;
using GraphManipulation.Models.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GraphManipulation.Models;

public class Purpose<T> : IModel<T>
{
    public T Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
}