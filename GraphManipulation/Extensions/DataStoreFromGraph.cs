using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Extensions;

public static class DataStoreFromGraph
{
    public static List<T> GetDataStores<T>(this IGraph graph) where T : DataStore
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
                if (typeof(T).IsSubclassOf(typeof(Relational)))
                {
                    graph.GetSchemas((datastore as Relational)!);
                }

                return datastore;
            })
            .ToList();
    }

    private static string GetNameOfNode(this IGraph graph, INode subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(subject, graph.CreateUriNode("ddl:hasName"))
            .Select(triple => triple.Object as LiteralNode)
            .First()!
            .Value;
    }

    private static void GetSchemas(this IGraph graph, Relational relational)
    {
        graph
            .GetSubStructures(relational)
            .ForEach(sub =>
            {
                var name = graph.GetNameOfNode(sub);
                var schema = new Schema(name);
                relational.AddStructure(schema);
                graph.GetTables(schema);
            });
    }

    private static List<INode> GetSubStructures(this IGraph graph, StructuredEntity subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(graph.CreateUriNode(subject.Uri), graph.CreateUriNode("ddl:hasStructure"))
            .Select(triple => triple.Object)
            .ToList();
    }
    
    private static void GetTables(this IGraph graph, Schema schema)
    {
        graph
            .GetSubStructures(schema)
            .ForEach(sub =>
            {
                var name = graph.GetNameOfNode(sub);
                var table = new Table(name);
                schema.AddStructure(table);
                graph.GetColumns(table);
            });
    }

    private static void GetColumns(this IGraph graph, Table table)
    {
        graph
            .GetSubStructures(table)
            .ForEach(sub =>
            {
                var name = graph.GetNameOfNode(sub);
                var column = new Column(name);
                table.AddStructure(column);
                column.SetDataType(graph.GetColumnDataType(column));
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