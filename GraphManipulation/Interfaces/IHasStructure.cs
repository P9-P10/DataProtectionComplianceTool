using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Interfaces;

public interface IHasStructure
{
    public void AddStructure(Structure structure);
}