using System.Linq;
using GraphManipulation.Extensions;
using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using Xunit;

namespace Test;

public class DataStoreFromGraphTest
{
    private const string baseUri = "http://www.test.com/";

    private static IGraph CreateBaseTestGraph()
    {
        IGraph graph = new Graph();

        graph.NamespaceMap.AddNamespace("ddl", GraphBased.OntologyNamespace);
        var uri = UriFactory.Create(baseUri);
        graph.BaseUri = uri;

        return graph;
    }


    [Fact]
    public void GetDataStoresCreatesTheListOfDataStoresOfGivenType()
    {
        var expectedSqlite = new Sqlite("TestSQLite", baseUri);

        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();

        Assert.Equal(expectedSqlite, actualSqlite);
        Assert.Equal(expectedSqlite.Uri, actualSqlite.Uri);
        Assert.Equal(expectedSqlite.BaseUri, actualSqlite.BaseUri);
        Assert.Equal(expectedSqlite.Name, actualSqlite.Name);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithSchemas()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName1 = "TestSchema1";
        const string expectedSchemaName2 = "TestSchema2";

        var expectedSqlite = new Sqlite(expectedSqliteName, baseUri);
        var expectedSchema1 = new Schema(expectedSchemaName1);
        var expectedSchema2 = new Schema(expectedSchemaName2);

        expectedSqlite.AddStructure(expectedSchema1);
        expectedSqlite.AddStructure(expectedSchema2);

        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);
        graph.AssertNamedEntityTriple(expectedSchema1);
        graph.AssertNamedEntityTriple(expectedSchema2);

        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema1);
        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema2);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();

        Assert.Equal(expectedSchema1, actualSqlite.FindSchema(expectedSchemaName1));
        Assert.Equal(expectedSchema1.Uri, actualSqlite.FindSchema(expectedSchemaName1).Uri);
        Assert.Equal(expectedSchema1.Name, actualSqlite.FindSchema(expectedSchemaName1).Name);

        Assert.Equal(expectedSchema2, actualSqlite.FindSchema(expectedSchemaName2));
        Assert.Equal(expectedSchema2.Uri, actualSqlite.FindSchema(expectedSchemaName2).Uri);
        Assert.Equal(expectedSchema2.Name, actualSqlite.FindSchema(expectedSchemaName2).Name);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTables()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName = "TestSchema";
        const string expectedTableName1 = "TestTable1";
        const string expectedTableName2 = "TestTable2";

        var expectedSqlite = new Sqlite(expectedSqliteName, baseUri);
        var expectedSchema = new Schema(expectedSchemaName);
        var expectedTable1 = new Table(expectedTableName1);
        var expectedTable2 = new Table(expectedTableName2);

        expectedSqlite.AddStructure(expectedSchema);
        expectedSchema.AddStructure(expectedTable1);
        expectedSchema.AddStructure(expectedTable2);

        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);
        graph.AssertNamedEntityTriple(expectedSchema);
        graph.AssertNamedEntityTriple(expectedTable1);
        graph.AssertNamedEntityTriple(expectedTable2);

        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema);
        graph.AssertHasStructureTriple(expectedSchema, expectedTable1);
        graph.AssertHasStructureTriple(expectedSchema, expectedTable2);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();

        Assert.Equal(expectedTable1, actualSqlite.FindSchema(expectedSchemaName).FindTable(expectedTableName1));
        Assert.Equal(expectedTable2, actualSqlite.FindSchema(expectedSchemaName).FindTable(expectedTableName2));
    }


    // TODO: Test with multiple SQLites in the same graph

    // TODO: Kunne man lave noget i stil med Datastore<Sqlite<Schema<Table<Column>>> ???
}