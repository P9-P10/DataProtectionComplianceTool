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
        
        if (from == to)
        {
            return;
        }

        var dataStore = GetDataStoreFromGraph();

        var structure = dataStore.Find<Structure>(from)
                        ?? throw new GraphManipulatorException("Could not find structure with Uri: " + from);

        var newName = to.ToString().Split(Entity.IdSeparator).Last();
        structure.UpdateName(newName);

        var newParentNames = to.ToString().Split(Entity.IdSeparator).SkipLast(1).ToList();
        var newParentString = string.Join(Entity.IdSeparator, newParentNames);
        var newParentUri = UriFactory.Create(newParentString);
        var newParentStructure = dataStore.Find<Structure>(newParentUri)
                                 ?? throw new GraphManipulatorException("Could not find parent structure with Uri: " +
                                                                        newParentUri);

        newParentStructure.AddStructure(structure);

        Changes.Add($"MOVE({from}, {to})");

        AddMoveChangesForSubStructures(from, structure.SubStructures);

        Graph = dataStore.ToGraph();
    }

    private void CheckMoveValidity(Uri from)
    {
        var typeTriple = Graph.GetTripleWithSubjectPredicateObject(
            Graph.CreateUriNode(from),
            Graph.CreateUriNode("rdf:type"),
            Graph.CreateUriNode(DataStoreDescriptionLanguage.Datastore));

        if (typeTriple is not null)
        {
            throw new GraphManipulatorException("Cannot move something of type Datastore");
        }
        
        var hasStructureTriples = Graph.GetTriplesWithPredicateObject(
            Graph.CreateUriNode(DataStoreDescriptionLanguage.HasStructure),
            Graph.CreateUriNode(from)
        ).ToList();

        if (!hasStructureTriples.Any())
        {
            throw new GraphManipulatorException("Cannot move structure without parent");
        }

        if (hasStructureTriples.Count > 1)
        {
            throw new GraphManipulatorException("Multiple parents found, cannot proceed");
        }

        var parentTypeTriple = Graph.GetTripleWithSubjectPredicateObject(
            hasStructureTriples.First().Subject,
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
        var structure = dataStore.Find<Structure>(uri) 
                        ?? throw new GraphManipulatorException("Could not find structure with Uri: " + uri);

        if (structure.Name == newName)
        {
            return;
        }

        structure.UpdateName(newName);
        Changes.Add($"RENAME({uri}, {structure.Uri})");

        AddMoveChangesForSubStructures(uri, structure.SubStructures);

        Graph = dataStore.ToGraph();
    }

    private void AddMoveChangesForSubStructures(Uri uri, IReadOnlyCollection<Structure> newSubstructures)
    {
        var dataStore = GetDataStoreFromGraph();
        var structure = dataStore.Find<Structure>(uri) 
                        ?? throw new GraphManipulatorException("Could not find structure with Uri: " + uri);

        foreach (var subUri in structure.SubStructures.Select(sub => sub.Uri))
        {
            var oldName = Graph.GetNameOfNode(Graph.CreateUriNode(subUri));
            var newSub = newSubstructures.First(sub => sub.Name == oldName);
            Changes.Add($"MOVE({subUri}, {newSub.Uri})");

            AddMoveChangesForSubStructures(subUri, newSub.SubStructures);
        }
    }

    private DataStore GetDataStoreFromGraph()
    {
        return Graph.ConstructDataStore<T>() 
               ?? throw new GraphManipulatorException("Could not construct datastore from graph");
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