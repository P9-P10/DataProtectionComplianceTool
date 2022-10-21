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

    public override IGraph ToGraph()
    {
        throw new NotImplementedException();
    }

    public override IGraphBased FromGraph(IGraph graph)
    {
        throw new NotImplementedException();
    }
}