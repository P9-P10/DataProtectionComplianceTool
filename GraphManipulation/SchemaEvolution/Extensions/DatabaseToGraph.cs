using GraphManipulation.SchemaEvolution.Models.Entity;
using GraphManipulation.SchemaEvolution.Models.Structures;
using GraphManipulation.SchemaEvolution.Ontologies;
using VDS.RDF;

namespace GraphManipulation.SchemaEvolution.Extensions;

public static class DatabaseToGraph
{
    public static IGraph ToGraph(this Entity entity)
    {
        IGraph graph = new Graph();

        graph.AddNameSpaces();
        graph.AddUriBase(entity);
        graph.AddEntityType(entity);

        if (entity is NamedEntity namedEntity)
        {
            graph.AddName(namedEntity);
        }

        if (entity is StructuredEntity structuredEntity)
        {
            graph.AddSubStructures(structuredEntity);
        }

        if (entity is Structure structure)
        {
            graph.AddHasDatabase(structure);
        }

        switch (entity)
        {
            case Table table:
                graph.AddPrimaryKeys(table);
                graph.AddForeignKeys(table);
                break;
            case Column column:
                graph.AddDataType(column);
                graph.AddIsNotNull(column);
                graph.AddOptions(column);
                break;
        }


        return graph;
    }

    private static void AddNameSpaces(this IGraph graph)
    {
        graph.NamespaceMap.AddNamespace(
            DatabaseDescriptionLanguage.OntologyPrefix,
            DatabaseDescriptionLanguage.OntologyUri);
    }

    private static void AddUriBase(this IGraph graph, Entity entity)
    {
        if (!entity.HasBase())
        {
            throw new DatabaseToGraphException("BaseUri was null when building graph");
        }

        var baseUri = UriFactory.Create(entity.BaseUri);
        graph.BaseUri = baseUri;
    }

    private static void AddEntityType(this IGraph graph, Entity entity)
    {
        graph.AssertTypeTriple(entity);
    }

    private static void AddName(this IGraph graph, NamedEntity entity)
    {
        graph.AssertNameTriple(entity);
    }

    private static void AddSubStructures(this IGraph graph, StructuredEntity entity)
    {
        foreach (var subStructure in entity.SubStructures)
        {
            graph.Merge(subStructure.ToGraph());

            graph.AssertHasStructureTriple(entity, subStructure);
        }
    }

    private static void AddHasDatabase(this IGraph graph, Structure structure)
    {
        if (!structure.HasDatabase())
        {
            throw new DatabaseToGraphException("Database was null when building graph");
        }

        graph.AssertHasDatabaseTriple(structure);
    }

    private static void AddPrimaryKeys(this IGraph graph, Table table)
    {
        graph.AssertPrimaryKeys(table);
    }

    private static void AddForeignKeys(this IGraph graph, Table table)
    {
        graph.AssertForeignKeys(table);
    }

    private static void AddDataType(this IGraph graph, Column column)
    {
        graph.AssertHasDataTypeTriple(column);
    }

    private static void AddIsNotNull(this IGraph graph, Column column)
    {
        graph.AssertIsNotNullTriple(column);
    }

    private static void AddOptions(this IGraph graph, Column column)
    {
        graph.AssertOptionsTriple(column);
    }
}

public class DatabaseToGraphException : Exception
{
    public DatabaseToGraphException(string message) : base(message)
    {
    }
}