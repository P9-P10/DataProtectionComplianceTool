using GraphManipulation.Models;
using VDS.RDF;

namespace GraphManipulation.Interfaces;

public interface IGraphBased
{
    
    public IGraph ToGraph();
    public GraphBased FromGraph(IGraph graph);
}