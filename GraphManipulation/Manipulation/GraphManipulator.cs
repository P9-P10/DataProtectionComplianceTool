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

    public void MoveToNewParent(Uri from, Structure parent)
    {
        throw new NotImplementedException();
    }

    public void Move(Uri from, Column to)
    {
        var dataStore = GetDataStoreFromGraph(); 

        var structure = dataStore.Find<Column>(from)!;
        var parentStructure = dataStore.Find<Table>(to.ParentStructure!)!;
        
        parentStructure.AddStructure(structure);

        Graph = dataStore.ToGraph();
        
        Changes.Add($"MOVE({from}, {to.Uri})");
    }

    public void Rename(Uri uri, string newName)
    {
        var dataStore = GetDataStoreFromGraph();
        var structure = dataStore.Find<Structure>(uri)!;
        
        structure.UpdateName(newName);
        Changes.Add($"RENAME({uri}, {structure.Uri})");

        AddMoveChangesForSubStructures(uri, structure.SubStructures);

        Graph = dataStore.ToGraph();
    }

    private void AddMoveChangesForSubStructures(Uri uri, IReadOnlyCollection<Structure> newSubstructures)
    {
        var dataStore = GetDataStoreFromGraph();
        var structure = dataStore.Find<Structure>(uri)!;
        
        var substructureUris = new List<string>(structure.SubStructures.Select(sub => sub.Uri.ToString()));
        
        foreach (var subUri in substructureUris.Select(UriFactory.Create))
        {
            var oldName = Graph.GetNameOfNode(Graph.CreateUriNode(subUri));
            var newSub = newSubstructures.First(sub => sub.Name == oldName);
            Changes.Add($"MOVE({subUri}, {newSub.Uri})");

            AddMoveChangesForSubStructures(subUri, newSub.SubStructures);
        }
    }

    private DataStore GetDataStoreFromGraph()
    {
        DataStore? dataStore = Graph.ConstructDataStore<T>();

        if (dataStore is null)
        {
            throw new GraphManipulatorException("Could not construct datastore from graph");
        }

        return dataStore;
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