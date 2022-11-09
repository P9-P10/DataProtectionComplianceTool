using AngleSharp.Text;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Extensions;

public static class DataStoreFromGraph
{
    public static List<T> ConstructDataStores<T>(this IGraph graph) where T : DataStore
    {
        return graph
            .GetTriplesWithObject(graph.CreateUriNode(GraphDataType.GetGraphTypeString(typeof(T))))
            .Select(triple =>
            {
                var name = graph.GetNameOfNode(triple.Subject);

                // TODO: Det her er langsomt, men jeg kunne ikke finde andre måder der virkede
                return (T)Activator.CreateInstance(typeof(T), name, graph.BaseUri.ToString())!;
            })
            .Select(datastore =>
            {
                if (datastore is Relational relational)
                {
                    graph.ConstructRelational(relational);
                }

                return datastore;
            })
            .ToList();
    }

    private static void ConstructRelational(this IGraph graph, Relational relational)
    {
        graph.ConstructSchemas(relational);
        graph.ConstructForeignKeys(relational);
    }

    private static string GetNameOfNode(this IGraph graph, INode subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(subject, graph.CreateUriNode("ddl:hasName"))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value;
    }

    private static void ConstructSchemas(this IGraph graph, Relational relational)
    {
        graph
            .GetSubStructures(relational)
            .ForEach(sub =>
            {
                var name = graph.GetNameOfNode(sub);
                var schema = new Schema(name);
                relational.AddStructure(schema);
                graph.ConstructTables(schema);
            });
    }

    private static List<INode> GetSubStructures(this IGraph graph, StructuredEntity subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(subject.Uri), graph.CreateUriNode("ddl:hasStructure"))
            .Select(triple => triple.Object)
            .ToList();
    }

    private static void ConstructTables(this IGraph graph, Schema schema)
    {
        graph
            .GetSubStructures(schema)
            .ForEach(sub =>
            {
                var name = graph.GetNameOfNode(sub);
                var table = new Table(name);
                schema.AddStructure(table);
                graph.ConstructColumns(table);
                graph.ConstructPrimaryKeys(table);
            });
    }

    private static void ConstructPrimaryKeys(this IGraph graph, Table table)
    {
        graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(table.Uri), graph.CreateUriNode("ddl:primaryKey"))
            .Select(triple => triple.Object as UriNode)
            .ToList()
            .ForEach(columnNode =>
            {
                var matchingColumn = table.SubStructures.First(sub => sub.Uri == columnNode!.Uri) as Column;
                table.AddPrimaryKey(matchingColumn!);
            });
    }

    private static Triple? GetTripleWithSubjectPredicateObject(this IGraph graph, INode subj, INode pred, INode obj)
    {
        return graph.Triples
            .FirstOrDefault(triple => triple.Subject.Equals(subj) &&
                                      triple.Predicate.Equals(pred) &&
                                      triple.Object.Equals(obj));
    }

    private static void ConstructForeignKeys(this IGraph graph, Relational relational)
    {
        graph
            .GetTriplesWithPredicate(graph.CreateUriNode("ddl:references"))
            .Where(triple =>
            {
                var subjStore = graph.GetTripleWithSubjectPredicateObject(
                    triple.Subject,
                    graph.CreateUriNode("ddl:hasStore"),
                    graph.CreateUriNode(relational.Uri));

                var objStore = graph.GetTripleWithSubjectPredicateObject(
                    triple.Object,
                    graph.CreateUriNode("ddl:hasStore"),
                    graph.CreateUriNode(relational.Uri));

                return subjStore is not null && objStore is not null;
            })
            .ToList()
            .ForEach(triple =>
            {
                var subj = (triple.Subject as UriNode)!;
                var obj = (triple.Object as UriNode)!;

                var from = relational.Find<Column>(subj.Uri)!;
                var to = relational.Find<Column>(obj.Uri)!;

                (from.ParentStructure as Table)!.AddForeignKey(from, to);
            });
    }

    private static void ConstructColumns(this IGraph graph, Table table)
    {
        graph
            .GetSubStructures(table)
            .ForEach(sub =>
            {
                var name = graph.GetNameOfNode(sub);
                var column = new Column(name);
                table.AddStructure(column);
                column.SetDataType(graph.GetColumnDataType(column));
                column.SetIsNotNull(graph.GetColumnIsNotNull(column));
                column.SetOptions(graph.GetColumnOptions(column));
            });
    }

    private static string GetColumnDataType(this IGraph graph, Column column)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(column.Uri), graph.CreateUriNode("ddl:hasDataType"))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value;
    }

    private static bool GetColumnIsNotNull(this IGraph graph, Column column)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(column.Uri), graph.CreateUriNode("ddl:isNotNull"))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value
            .ToBoolean();
    }

    private static string GetColumnOptions(this IGraph graph, Column column)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(column.Uri), graph.CreateUriNode("ddl:columnOptions"))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value;
    }


    // https://dotnetrdf.org/docs/stable/user_guide/Querying-With-SPARQL.html
    // var parser = new SparqlQueryParser();
    // var query = parser.ParseFromString("SELECT ?relational WHERE { ?relational a ddl:Relational }");
    //
    // TripleStore tripleStore = new TripleStore();
    // tripleStore.Add(graph);
    //
    // InMemoryDataset dataset = new InMemoryDataset(tripleStore);
    //
    // var processor = new LeviathanQueryProcessor(dataset);
    //
    // var results = processor.ProcessQuery(query);
}