using System.Security.Cryptography;

namespace GraphManipulation.Models;

public abstract class NamedEntity : Entity
{
    public string Name { get; }

    public NamedEntity(string name) : base(name)
    {
        Name = name;
    }
}