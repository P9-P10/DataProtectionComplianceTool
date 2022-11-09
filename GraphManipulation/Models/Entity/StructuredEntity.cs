using GraphManipulation.Extensions;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Models.Entity;

public abstract class StructuredEntity : NamedEntity
{
    public List<Structure> SubStructures = new();

    protected StructuredEntity(string name) : base(name)
    {
    }

    public abstract void AddStructure(Structure structure);
}