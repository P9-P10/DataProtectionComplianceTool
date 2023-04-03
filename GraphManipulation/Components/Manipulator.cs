using GraphManipulation.Extensions;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using GraphManipulation.Ontologies;
using VDS.RDF;

namespace GraphManipulation.Components;

public class Manipulator<T> where T : Database
{
    public Manipulator(IGraph graph)
    {
        Graph = new Graph();
        Graph.BaseUri = graph.BaseUri;
        Graph.Merge(graph);
        Changes = new List<string>();
    }

    public IGraph Graph { get; private set; }
    public List<string> Changes { get; }
    

    public void Move(Uri from, Uri to)
    {
        CheckMoveValidity(from, to);

        if (from == to)
        {
            return;
        }

        var database = GetDatabaseFromGraph();

        var structure = database.Find<Structure>(from)
                        ?? throw new ManipulatorException("Could not find structure with Uri: " + from);

        var newName = to.ToString().Split(Entity.IdSeparator).Last();
        structure.UpdateName(newName);

        var newParentUri = ExtractParentUriFromChildUri(to);
        var newParentStructure = database.Find<Structure>(newParentUri)
                                 ?? throw new ManipulatorException("Could not find parent structure with Uri: " +
                                                                   newParentUri);

        newParentStructure.AddStructure(structure);

        Changes.Add($"MOVE({from}, {to})");

        AddMoveChangesForSubStructures(from, structure.SubStructures);

        Graph = database.ToGraph();
    }

    private void CheckMoveValidity(Uri from, Uri to)
    {
        if (ExtractNameFromUri(from) != ExtractNameFromUri(to))
        {
            throw new ManipulatorException("MOVE cannot change the name of an entity, use RENAME");
        }

        var typeTriple = Graph.GetTripleWithSubjectPredicateObject(
            Graph.CreateUriNode(from),
            Graph.CreateUriNode("rdf:type"),
            Graph.CreateUriNode(DatabaseDescriptionLanguage.Database));

        if (typeTriple is not null)
        {
            throw new ManipulatorException("Cannot move something of type Database");
        }

        var hasStructureTriples = Graph.GetTriplesWithPredicateObject(
            Graph.CreateUriNode(DatabaseDescriptionLanguage.HasStructure),
            Graph.CreateUriNode(from)
        ).ToList();

        if (!hasStructureTriples.Any())
        {
            throw new ManipulatorException("Cannot move structure without parent");
        }

        if (hasStructureTriples.Count > 1)
        {
            throw new ManipulatorException("Multiple parents found, cannot proceed");
        }

        var parentTypeTriple = Graph.GetTripleWithSubjectPredicateObject(
            hasStructureTriples.First().Subject,
            Graph.CreateUriNode("rdf:type"),
            Graph.CreateUriNode(DatabaseDescriptionLanguage.Database)
        );

        if (parentTypeTriple is not null)
        {
            throw new ManipulatorException("Cannot move structure whose parent is a Database");
        }
    }

    public void Rename(Uri from, Uri to)
    {
        CheckRenameValidity(from, to);

        if (from == to)
        {
            return;
        }

        var database = GetDatabaseFromGraph();
        var structure = database.Find<Structure>(from)
                        ?? throw new ManipulatorException("Could not find structure with Uri: " + from);

        structure.UpdateName(ExtractNameFromUri(to));
        Changes.Add($"RENAME({from}, {structure.Uri})");

        AddMoveChangesForSubStructures(from, structure.SubStructures);

        Graph = database.ToGraph();
    }

    private void CheckRenameValidity(Uri from, Uri to)
    {
        if (ExtractParentUriFromChildUri(from) != ExtractParentUriFromChildUri(to))
        {
            throw new ManipulatorException("RENAME cannot move an entity, use MOVE instead");
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
        var database = GetDatabaseFromGraph();
        var structure = database.Find<Structure>(uri)
                        ?? throw new ManipulatorException("Could not find structure with Uri: " + uri);

        foreach (var subUri in structure.SubStructures.Select(sub => sub.Uri))
        {
            var oldName = Graph.GetNameOfNode(Graph.CreateUriNode(subUri));
            var newSub = newSubstructures.First(sub => sub.Name == oldName);
            Changes.Add($"MOVE({subUri}, {newSub.Uri})");

            AddMoveChangesForSubStructures(subUri, newSub.SubStructures);
        }
    }

    private Database GetDatabaseFromGraph()
    {
        return Graph.ConstructDatabase<T>()
               ?? throw new ManipulatorException("Could not construct database from graph");
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}

public class ManipulatorException : Exception
{
    public ManipulatorException(string message) : base(message)
    {
    }
}