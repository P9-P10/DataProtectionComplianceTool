using GraphManipulation.Interfaces;
using GraphManipulation.Models.Connections;
using VDS.RDF;

namespace GraphManipulation.Models.Stores;

public class Sqlite : Relational
{
    public Sqlite(string name) : base(name)
    {
    }

    public override Connection GetConnection()
    {
        throw new NotImplementedException();
    }

    // protected override IGraph ComputeGraph()
    // {
    //     IGraph graph = new Graph();
    //
    //     foreach (var structure in Structures)
    //     {
    //         graph.Merge(structure.ToGraph());
    //     }
    //
    //     return graph;
    // }

    public new IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();
        return graph;
    }

    protected override string GetGraphTypeString()
    {
        return "SQLite";
    }
}