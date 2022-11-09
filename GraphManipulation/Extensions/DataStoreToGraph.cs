using GraphManipulation.Models;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace GraphManipulation.Extensions;

public static class DataStoreToGraph
{
    public static Uri OntologyNamespace =>
        UriFactory.Create("http://www.cs-22-dt-9-03.org/datastore-description-language#");
    
    public static IGraph ToGraph(this Entity entity)
    {
        IGraph graph = new Graph();
        
        graph.AddNameSpaces();
        graph.AddUriBase(entity);
        graph.AddEntityType(entity);

        if (entity.GetType().IsSubclassOf(typeof(NamedEntity)))
        {
            graph.AddName(entity as NamedEntity);
        }

        if (entity.GetType().IsSubclassOf(typeof(StructuredEntity)))
        {
            graph.AddSubStructures(entity as StructuredEntity);
        }

        if (entity.GetType().IsSubclassOf(typeof(Structure)))
        {
            graph.AddHasStore(entity as Structure);
        }

        if (entity is Table table)
        {
            graph.AddPrimaryKeys(table);
            graph.AddForeignKeys(table);
        }

        if (entity is Column column)
        {
            graph.AddDataType(column);
            graph.AddIsNotNull(column);
            graph.AddOptions(column);
        }
        

        return graph;
    }

    private static void AddNameSpaces(this IGraph graph)
    {
        graph.NamespaceMap.AddNamespace("ddl", OntologyNamespace);
    }

    private static void AddUriBase(this IGraph graph, Entity entity)
    {
        if (entity.HasBase())
        {
            throw new DataStoreToGraphException("BaseUri was null when building graph");
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

    private static void AddHasStore(this IGraph graph, Structure structure)
    {
        if (structure.HasStore())
        {
            throw new DataStoreToGraphException("Store was null when building graph");
        }

        var triple = new Triple(
            graph.CreateUriNode(structure.Uri),
            graph.CreateUriNode("ddl:hasStore"),
            graph.CreateUriNode(structure.Store.Uri));

        graph.Assert(triple);
    }

    private static void AddPrimaryKeys(this IGraph graph, Table table)
    {
        if (table.PrimaryKeys.Count == 0)
        {
            throw new DataStoreToGraphException("No primary keys when creating graph");
        }

        var subj = graph.CreateUriNode(table.Uri);
        var pred = graph.CreateUriNode("ddl:primaryKey");

        foreach (var primaryKey in table.PrimaryKeys)
        {
            var obj = graph.CreateUriNode(primaryKey.Uri);
            graph.Assert(subj, pred, obj);
        }
    }

    private static void AddForeignKeys(this IGraph graph, Table table)
    {
        if (table.ForeignKeys.Count == 0)
        {
            return;
        }

        var tableUri = graph.CreateUriNode(table.Uri);
        var foreignKeyPredicate = graph.CreateUriNode("ddl:foreignKey");
        var referencesPredicate = graph.CreateUriNode("ddl:references");
        var foreignKeyOnDeletePredicate = graph.CreateUriNode("ddl:foreignKeyOnDelete");
        var foreignKeyOnUpdatePredicate = graph.CreateUriNode("ddl:foreignKeyOnUpdate");

        foreach (var foreignKey in table.ForeignKeys)
        {
            var from = graph.CreateUriNode(foreignKey.From.Uri);
            var to = graph.CreateUriNode(foreignKey.To.Uri);

            graph.Assert(tableUri, foreignKeyPredicate, from);
            graph.Assert(from, referencesPredicate, to);

            var onDelete = graph.CreateLiteralNode(foreignKey.OnDeleteString);
            var onUpdate = graph.CreateLiteralNode(foreignKey.OnUpdateString);

            graph.Assert(from, foreignKeyOnDeletePredicate, onDelete);
            graph.Assert(from, foreignKeyOnUpdatePredicate, onUpdate);
        }
    }

    private static void AddDataType(this IGraph graph, Column column)
    {
        var triple = new Triple(
            graph.CreateUriNode(column.Uri),
            graph.CreateUriNode("ddl:hasDataType"),
            graph.CreateLiteralNode(column.DataType)
        );

        graph.Assert(triple);
    }

    private static void AddIsNotNull(this IGraph graph, Column column)
    {
        var triple = new Triple(
            graph.CreateUriNode(column.Uri),
            graph.CreateUriNode("ddl:isNotNull"),
            graph.CreateLiteralNode(column.IsNotNull.ToString().ToLower(),
                UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeBoolean)));

        graph.Assert(triple);
    }

    private static void AddOptions(this IGraph graph, Column column)
    {
        var triple = new Triple(
            graph.CreateUriNode(column.Uri),
            graph.CreateUriNode("ddl:columnOptions"),
            graph.CreateLiteralNode(column.Options));

        graph.Assert(triple);
    }
}

public class DataStoreToGraphException : Exception
{
    public DataStoreToGraphException(string message) : base(message)
    {
    }
}