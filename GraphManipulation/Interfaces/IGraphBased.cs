using VDS.RDF;

namespace GraphManipulation.Interfaces;

public interface IGraphBased
{
    
    public IGraph ToGraph();
    public IGraphBased FromGraph(IGraph graph);
}