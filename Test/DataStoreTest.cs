using System.Text;
using System.Xml;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class DataStoreTest
{
    private const string baseUri = "http://www.test.com/";

    [Fact]
    public void UpdateBaseSetsBase()
    {
        var sqlite = new Sqlite("SQLite");

        sqlite.UpdateBaseUri(baseUri);

        Assert.Equal(baseUri, sqlite.BaseUri);
    }

    [Fact]
    public void AddStructureAddsGivenStructureToListOfStructures()
    {
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.UpdateBaseUri(baseUri);
        sqlite.AddStructure(schema);
        Assert.Contains(schema, sqlite.SubStructures);
    }

    [Fact]
    public void StructureCannotBeAddedMultipleTimesToStoreListOfStructures()
    {
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.UpdateBaseUri(baseUri);
        sqlite.AddStructure(schema);
        sqlite.AddStructure(schema);

        Assert.Single(sqlite.SubStructures);
    }

    [Fact]
    public void UpdateBaseUpdatesStructuresBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.UpdateBaseUri(baseUri + "/Test/");
        sqlite.AddStructure(table);
        sqlite.UpdateBaseUri(baseUri + "/Expected/");

        Assert.Equal(baseUri + "/Expected/", table.BaseUri);
        Assert.Equal(sqlite.BaseUri, table.BaseUri);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndStructuresId()
    {
        const string baseNamespace = baseUri + "/Expected/";
        const string sqliteName = "SQLite";
        const string tableName = "Table";

        var expectedSqliteString = baseNamespace + sqliteName;
        var expectedTableString = expectedSqliteString + Entity.IdSeparator + tableName;

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);

        sqlite.UpdateBaseUri(baseUri + "/Test/");
        sqlite.AddStructure(table);
        sqlite.UpdateBaseUri(baseNamespace);

        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
    }

    [Fact]
    public void AddStructureUpdatesBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        const string expected = baseUri + "/Expected/";
        sqlite.UpdateBaseUri(expected);
        sqlite.AddStructure(table);

        Assert.Equal(expected, sqlite.BaseUri);
        Assert.Equal(sqlite.BaseUri, table.BaseUri);
    }

    [Fact]
    public void AddStructureUpdatesStore()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.UpdateBaseUri(baseUri);
        sqlite.AddStructure(table);

        Assert.Equal(sqlite, table.Store);
    }
}