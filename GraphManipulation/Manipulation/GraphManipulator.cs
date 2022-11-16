using GraphManipulation.Extensions;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using GraphManipulation.Ontologies;
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

    public void Move(Uri from, Uri to)
    {
        CheckMoveValidity(from);

        var dataStore = GetDataStoreFromGraph();

        var structure = dataStore.Find<Structure>(from)!;

        var newParentUri = UriFactory.Create(string.Join(Entity.IdSeparator, to.ToString().Split(Entity.IdSeparator).SkipLast(1)));

        var parentStructure = dataStore.Find<Structure>(newParentUri)!;
        
        parentStructure.AddStructure(structure);

        Changes.Add($"MOVE({from}, {to})");
        
        AddMoveChangesForSubStructures(from, structure.SubStructures);
        
        Graph = dataStore.ToGraph();
    }

    private void CheckMoveValidity(Uri from)
    {
        var triples = Graph.GetTriplesWithPredicateObject(
            Graph.CreateUriNode(DataStoreDescriptionLanguage.HasStructure),
            Graph.CreateUriNode(from)
        ).ToList();

        if (!triples.Any())
        {
            throw new GraphManipulatorException("Cannot move structure without parent");
        }
        
        if (triples.Count > 1)
        {
            throw new GraphManipulatorException("Multiple parents found, something went wrong");
        }

        var parentTypeTriple = Graph.GetTripleWithSubjectPredicateObject(
            triples.First().Subject,
            Graph.CreateUriNode("rdf:type"),
            Graph.CreateUriNode(DataStoreDescriptionLanguage.Datastore)
        );

        if (parentTypeTriple is not null)
        {
            throw new GraphManipulatorException("Cannot move structure whose parent is a Datastore");
        }
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