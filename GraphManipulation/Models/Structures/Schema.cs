using GraphManipulation.Interfaces;
using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Schema : Structure
{
    public Schema(string name) : base(name)
    {
    }

    public override IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();
        return graph;
    }

    protected override string GetGraphTypeString()
    {
        return "Schema";
    }
}