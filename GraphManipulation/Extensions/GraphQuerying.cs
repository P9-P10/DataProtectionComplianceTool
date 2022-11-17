using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Structures;
using GraphManipulation.Ontologies;
using VDS.RDF;
using AngleSharp.Text;

namespace GraphManipulation.Extensions;

public static class GraphQuerying
{
    public static Triple? GetTripleWithSubjectPredicateObject(this IGraph graph, INode subj, INode pred, INode obj)
    {
        return graph.Triples
            .FirstOrDefault(triple => triple.Subject.Equals(subj) &&
                                      triple.Predicate.Equals(pred) &&
                                      triple.Object.Equals(obj));
    }
    
    public static string GetNameOfNode(this IGraph graph, INode subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(subject, graph.CreateUriNode(DataStoreDescriptionLanguage.HasName))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value;
    }
    
    public static IEnumerable<INode> GetSubStructures(this IGraph graph, StructuredEntity subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(
                graph.CreateUriNode(subject.Uri), 
                graph.CreateUriNode(DataStoreDescriptionLanguage.HasStructure))
            .Select(triple => triple.Object);
    }

    public static IEnumerable<T> GetSubStructures<T>(this IGraph graph, StructuredEntity subject) where T : Structure
    {
        return graph
            .GetSubStructures(subject)
            .Select(graph.GetNameOfNode)
            // TODO: Det her er åbenbart langsomt, men jeg kunne ikke finde andre måder der virkede
            .Select(name => (T)Activator.CreateInstance(typeof(T), name)!);
    }
    
    public static string? GetDataStoreDescriptionLanguageTypeFromUri(this IGraph graph, Uri uri)
    {
        var triples = graph.GetTriplesWithSubjectPredicate(
            graph.CreateUriNode(uri),
            graph.CreateUriNode("rdf:type")).ToList();

        return triples.Count == 0 ? 
            null : 
            (triples.First().Object as UriNode)!
            .Uri
            .ToString()
            .Replace(
                DataStoreDescriptionLanguage.OntologyUri.ToString(), 
                DataStoreDescriptionLanguage.OntologyPrefix + ":");
    }
    
    public static string GetColumnDataType(this IGraph graph, Column column)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(column.Uri), graph.CreateUriNode(DataStoreDescriptionLanguage.HasDataType))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value;
    }

    public static bool GetColumnIsNotNull(this IGraph graph, Column column)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(column.Uri), graph.CreateUriNode(DataStoreDescriptionLanguage.IsNotNull))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value
            .ToBoolean();
    }

    public static string GetColumnOptions(this IGraph graph, Column column)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(column.Uri), graph.CreateUriNode(DataStoreDescriptionLanguage.ColumnOptions))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value;
    }
}