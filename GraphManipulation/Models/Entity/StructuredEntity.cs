using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Entity;

public abstract class StructuredEntity : NamedEntity
{
    public readonly List<Structure> SubStructures = new();

    protected StructuredEntity(string name) : base(name)
    {
    }

    public abstract void AddStructure(Structure structure);
}