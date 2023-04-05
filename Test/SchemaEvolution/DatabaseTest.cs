using GraphManipulation.SchemaEvolution.Models.Entity;
using GraphManipulation.SchemaEvolution.Models.Stores;
using GraphManipulation.SchemaEvolution.Models.Structures;
using Xunit;

namespace Test.SchemaEvolution;

public class DatabaseTest
{
    private const string BaseUri = "http://www.test.com/";

    [Fact]
    public void UpdateBaseSetsBase()
    {
        var sqlite = new Sqlite("SQLite");

        sqlite.UpdateBaseUri(BaseUri);

        Assert.Equal(BaseUri, sqlite.BaseUri);
    }

    [Fact]
    public void AddStructureAddsGivenStructureToListOfStructures()
    {
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.UpdateBaseUri(BaseUri);
        sqlite.AddStructure(schema);
        Assert.Contains(schema, sqlite.SubStructures);
    }

    [Fact]
    public void StructureCannotBeAddedMultipleTimesToDatabasesListOfStructures()
    {
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.UpdateBaseUri(BaseUri);
        sqlite.AddStructure(schema);
        sqlite.AddStructure(schema);

        Assert.Single(sqlite.SubStructures);
    }

    [Fact]
    public void UpdateBaseUpdatesStructuresBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.UpdateBaseUri(BaseUri + "/Test/");
        sqlite.AddStructure(table);
        sqlite.UpdateBaseUri(BaseUri + "/Expected/");

        Assert.Equal(BaseUri + "/Expected/", table.BaseUri);
        Assert.Equal(sqlite.BaseUri, table.BaseUri);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndStructuresId()
    {
        const string baseNamespace = BaseUri + "/Expected/";
        const string sqliteName = "SQLite";
        const string tableName = "Table";

        var expectedSqliteString = baseNamespace + sqliteName;
        var expectedTableString = expectedSqliteString + Entity.IdSeparator + tableName;

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);

        sqlite.UpdateBaseUri(BaseUri + "/Test/");
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

        const string expected = BaseUri + "/Expected/";
        sqlite.UpdateBaseUri(expected);
        sqlite.AddStructure(table);

        Assert.Equal(expected, sqlite.BaseUri);
        Assert.Equal(sqlite.BaseUri, table.BaseUri);
    }

    [Fact]
    public void AddStructureUpdatesDatabase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.UpdateBaseUri(BaseUri);
        sqlite.AddStructure(table);

        Assert.Equal(sqlite, table.Database);
    }
}