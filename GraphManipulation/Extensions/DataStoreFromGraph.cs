using System.Linq.Expressions;
using System.Reflection;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using GraphManipulation.Extensions;

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
                    graph.GetSchemas((datastore as Relational)!).ForEach(datastore.AddStructure);
                }

                return datastore;
            })
            .ToList();
    }

    private static string GetNameOfNode(this IGraph graph, INode subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(subject, graph.CreateUriNode("ddl:hasName"))
            .Select(nodeTriple => nodeTriple.Object as LiteralNode)
            .First()!
            .Value;
    }

    private static List<Schema> GetSchemas(this IGraph graph, Relational relational)
    {
        return graph
            .GetSubStructures(graph.CreateUriNode(relational.Uri))
            .Select(sub =>
            {
                var name = graph.GetNameOfNode(sub);
                return new Schema(name);
            })
            .Select(schema =>
            {
                // graph.GetTa
                return schema;
            })
            .ToList();
    }

    private static List<INode> GetSubStructures(this IGraph graph, INode subject)
    {
        return graph
            .GetTriplesWithSubjectPredicate(subject, graph.CreateUriNode("ddl:hasStructure"))
            .Select(triple => triple.Object)
            .ToList();
    }
    
    //
    // private static List<Table> GetTables(this IGraph graph, Schema schema)
    // {
    //     
    // }
    //
    // private static List<Column> GetColumns(this IGraph graph, Table table)
    // {
    //     
    // }


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