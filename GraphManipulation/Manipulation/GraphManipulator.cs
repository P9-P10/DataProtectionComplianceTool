using System.Text.RegularExpressions;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using GraphManipulation.Ontologies;
using VDS.RDF;

namespace GraphManipulation.Manipulation;

public class GraphManipulator<T> where T : DataStore
{
    public GraphManipulator(IGraph graph)
    {
        Graph = new Graph();
        Graph.BaseUri = graph.BaseUri;
        Graph.Merge(graph);
        Changes = new List<string>();
    }

    public IGraph Graph { get; private set; }
    public List<string> Changes { get; }

    private static string ValidManipulationQueryPattern => "^(\\w+)\\(([\\w:#\\/.]+),\\s?([\\w:#\\/.]+)\\)$";

    public void Move(Uri from, Uri to)
    {
        CheckMoveValidity(from, to);

        if (from == to)
        {
            return;
        }

        var dataStore = GetDataStoreFromGraph();

        var structure = dataStore.Find<Structure>(from)
                        ?? throw new GraphManipulatorException("Could not find structure with Uri: " + from);

        var newName = to.ToString().Split(Entity.IdSeparator).Last();
        structure.UpdateName(newName);

        var newParentUri = ExtractParentUriFromChildUri(to);
        var newParentStructure = dataStore.Find<Structure>(newParentUri)
                                 ?? throw new GraphManipulatorException("Could not find parent structure with Uri: " +
                                                                        newParentUri);

        newParentStructure.AddStructure(structure);

        Changes.Add($"MOVE({from}, {to})");

        AddMoveChangesForSubStructures(from, structure.SubStructures);

        Graph = dataStore.ToGraph();
    }

    private void CheckMoveValidity(Uri from, Uri to)
    {
        if (ExtractNameFromUri(from) != ExtractNameFromUri(to))
        {
            throw new GraphManipulatorException("MOVE cannot change the name of an entity, use RENAME");
        }

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

    public void Rename(Uri from, Uri to)
    {
        CheckRenameValidity(from, to);

        if (from == to)
        {
            return;
        }

        var dataStore = GetDataStoreFromGraph();
        var structure = dataStore.Find<Structure>(from)
                        ?? throw new GraphManipulatorException("Could not find structure with Uri: " + from);

        structure.UpdateName(ExtractNameFromUri(to));
        Changes.Add($"RENAME({from}, {structure.Uri})");

        AddMoveChangesForSubStructures(from, structure.SubStructures);

        Graph = dataStore.ToGraph();
    }

    private void CheckRenameValidity(Uri from, Uri to)
    {
        if (ExtractParentUriFromChildUri(from) != ExtractParentUriFromChildUri(to))
        {
            throw new GraphManipulatorException("RENAME cannot move an entity, use MOVE instead");
        }
    }

    private static Uri ExtractParentUriFromChildUri(Uri childUri)
    {
        var parentNames = childUri.ToString().Split(Entity.IdSeparator).SkipLast(1).ToList();
        var parentString = string.Join(Entity.IdSeparator, parentNames);
        var parentUri = UriFactory.Create(parentString);

        return parentUri;
    }

    private static string ExtractNameFromUri(Uri uri)
    {
        return uri.ToString().Split(Entity.IdSeparator).Last();
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

    public bool IsValidManipulationQuery(string query)
    {
        return Regex.IsMatch(query, ValidManipulationQueryPattern);
    }

    public void ApplyManipulationQuery(string query)
    {
        var match = Regex.Match(query, ValidManipulationQueryPattern);

        var command = match.Groups[1].ToString().ToUpper();
        var firstUri = new Uri(match.Groups[2].ToString());
        var secondUri = new Uri(match.Groups[3].ToString());

        Action<Uri, Uri> action = command switch
        {
            "MOVE" => Move,
            "RENAME" => Rename,
            _ => throw new GraphManipulatorException("Command not supported")
        };

        action(firstUri, secondUri);
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