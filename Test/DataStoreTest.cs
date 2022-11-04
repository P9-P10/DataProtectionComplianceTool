using System.Text;
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
        const string baseName = "Test";

        sqlite.UpdateBaseUri(baseName);

        Assert.Equal(baseName, sqlite.BaseUri);
    }

    [Fact]
    public void AddStructureAddsGivenStructureToListOfStructures()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.UpdateBaseUri(baseNamespace);
        sqlite.AddStructure(schema);
        Assert.Contains(schema, sqlite.SubStructures);
    }

    [Fact]
    public void StructureCannotBeAddedMultipleTimesToStoreListOfStructures()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.UpdateBaseUri(baseNamespace);
        sqlite.AddStructure(schema);
        sqlite.AddStructure(schema);

        Assert.Single(sqlite.SubStructures);
    }

    [Fact]
    public void UpdateBaseUpdatesStructuresBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.UpdateBaseUri("Test");
        sqlite.AddStructure(table);
        sqlite.UpdateBaseUri("Expected");

        Assert.Equal("Expected", table.BaseUri);
        Assert.Equal(sqlite.BaseUri, table.BaseUri);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndStructuresId()
    {
        const string baseNamespace = "Expected";
        const string sqliteName = "SQLite";
        const string tableName = "Table";

        var algorithm = EntityTest.GetHashAlgorithm();

        const string sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Entity.HashToId(expectedSqliteHash);

        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Entity.HashToId(expectedTableHash);

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);

        sqlite.UpdateBaseUri("Test");
        sqlite.AddStructure(table);
        sqlite.UpdateBaseUri("Expected");

        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
    }

    [Fact]
    public void AddStructureUpdatesBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        var expected = "Expected";
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

        sqlite.UpdateBaseUri("Test");
        sqlite.AddStructure(table);

        Assert.Equal(sqlite, table.Store);
    }

    public class ToGraphTest
    {
    }

    public class FromGraphTest
    {
    }
}