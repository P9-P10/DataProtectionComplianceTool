using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Manipulation;

public class GraphManipulator<T> where T : DataStore
{
    public IGraph Graph { get; private set; }
    public List<string> Changes { get; }
    
    public GraphManipulator(IGraph graph)
    {
        Graph = new Graph();
        Graph.BaseUri = graph.BaseUri;
        Graph.Merge(graph);
        Changes = new List<string>();
    }

    public void Move(Uri from, Column to)
    {
        DataStore? dataStore = Graph.ConstructDataStore<T>();

        if (dataStore is null)
        {
            throw new GraphManipulatorException("Could not construct datastore from graph");
        }
        
        var structure = dataStore.Find<Column>(from)!;
        var parentStructure = dataStore.Find<Table>(to.ParentStructure!)!;
        
        parentStructure.AddStructure(structure);

        Graph = dataStore.ToGraph();
        
        Changes.Add($"MOVE({from}, {to.Uri})");
    }

    public void Rename(Uri uri, string newName)
    {
        throw new NotImplementedException();
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}

public class GraphManipulatorException : Exception
{
    public GraphManipulatorException(string message) : base(message)
    {
    }
}