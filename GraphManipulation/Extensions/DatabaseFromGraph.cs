using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using GraphManipulation.Ontologies;
using VDS.RDF;

namespace GraphManipulation.Extensions;

public static class DatabaseFromGraph
{
    public static T? ConstructDatabase<T>(this IGraph graph) where T : Database
    {
        return graph
            .GetTriplesWithObject(graph.CreateUriNode(GraphDataType.GetGraphTypeString(typeof(T))))
            .Select(triple =>
            {
                var name = graph.GetNameOfNode(triple.Subject);

                // TODO: Det her er åbenbart langsomt, men jeg kunne ikke finde andre måder der virkede
                return (T)Activator.CreateInstance(typeof(T), name, graph.BaseUri.ToString())!;
            })
            .Select(database =>
            {
                if (database is Relational relational)
                {
                    graph.ConstructRelational(relational);
                }

                return database;
            })
            .FirstOrDefault();
    }

    private static void ConstructRelational(this IGraph graph, Relational relational)
    {
        graph.ConstructSchemas(relational);
        graph.ConstructForeignKeys(relational);
    }

    private static void ConstructSchemas(this IGraph graph, Relational relational)
    {
        foreach (var schema in graph.GetSubStructures<Schema>(relational))
        {
            relational.AddStructure(schema);
            graph.ConstructTables(schema);
        }
    }

    private static void ConstructTables(this IGraph graph, Schema schema)
    {
        foreach (var table in graph.GetSubStructures<Table>(schema))
        {
            schema.AddStructure(table);
            graph.ConstructColumns(table);
            graph.ConstructPrimaryKeys(table);
        }
    }

    private static void ConstructPrimaryKeys(this IGraph graph, Table table)
    {
        foreach (var columnNode in graph
                     .GetTriplesWithSubjectPredicate(
                         graph.CreateUriNode(table.Uri),
                         graph.CreateUriNode(DatabaseDescriptionLanguage.PrimaryKey))
                     .Select(triple => (triple.Object as UriNode)!))
        {
            var matchingColumn = table.SubStructures.First(sub => sub.Uri == columnNode.Uri) as Column;
            table.AddPrimaryKey(matchingColumn!);
        }
    }

    private static void ConstructForeignKeys(this IGraph graph, Relational relational)
    {
        var triples = graph
            .GetTriplesWithPredicate(graph.CreateUriNode(DatabaseDescriptionLanguage.References))
            .Where(triple =>
            {
                var subjDatabase = graph.GetTripleWithSubjectPredicateObject(
                    triple.Subject,
                    graph.CreateUriNode(DatabaseDescriptionLanguage.HasDatabase),
                    graph.CreateUriNode(relational.Uri));

                var objDatabase = graph.GetTripleWithSubjectPredicateObject(
                    triple.Object,
                    graph.CreateUriNode(DatabaseDescriptionLanguage.HasDatabase),
                    graph.CreateUriNode(relational.Uri));

                return subjDatabase is not null && objDatabase is not null;
            });

        foreach (var triple in triples)
        {
            var subj = (triple.Subject as UriNode)!;
            var obj = (triple.Object as UriNode)!;

            var from = relational.Find<Column>(subj.Uri)!;
            var to = relational.Find<Column>(obj.Uri)!;

            (from.ParentStructure as Table)!.AddForeignKey(from, to);
        }
    }

    private static void ConstructColumns(this IGraph graph, Table table)
    {
        foreach (var column in graph.GetSubStructures<Column>(table))
        {
            table.AddStructure(column);
            column.SetDataType(graph.GetColumnDataType(column));
            column.SetIsNotNull(graph.GetColumnIsNotNull(column));
            column.SetOptions(graph.GetColumnOptions(column));
        }
    }
}