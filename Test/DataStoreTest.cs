using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class DataStoreTest
{
    [Fact]
    public void SetBaseSetsBase()
    {
        var sqlite = new Sqlite("SQLite");
        const string baseName = "Test";

        sqlite.SetBase(baseName);

        Assert.Equal(baseName, sqlite.Base);
    }

    [Fact]
    public void AddStructureAddsGivenStructureToListOfStructures()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.SetBase(baseNamespace);
        sqlite.AddStructure(schema);
        Assert.Contains(schema, sqlite.Structures);
    }

    [Fact]
    public void StructureCannotBeAddedMultipleTimesToStoreListOfStructures()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);

        sqlite.SetBase(baseNamespace);
        sqlite.AddStructure(schema);
        sqlite.AddStructure(schema);

        Assert.Single(sqlite.Structures);
    }
}