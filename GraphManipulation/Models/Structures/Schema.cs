using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Schema : Structure
{
    public Schema(string name) : base(name)
    {
    }

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();
        return graph;
    }
}