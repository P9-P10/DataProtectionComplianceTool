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

    public override IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();
        return graph;
    }

    protected override string GetGraphTypeString()
    {
        return "SQLite";
    }
}