using System.Security.Cryptography;
using System.Text;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class DataStoreTest
{
    [Fact]
    public void UpdateBaseSetsBase()
    {
        var sqlite = new Sqlite("SQLite");
        const string baseName = "Test";

        sqlite.UpdateBase(baseName);

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

        sqlite.UpdateBase(baseNamespace);
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

        sqlite.UpdateBase(baseNamespace);
        sqlite.AddStructure(schema);
        sqlite.AddStructure(schema);

        Assert.Single(sqlite.SubStructures);
    }

    [Fact]
    public void UpdateBaseUpdatesStructuresBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        
        sqlite.UpdateBase("Test");
        sqlite.AddStructure(table);
        sqlite.UpdateBase("Expected");
        
        Assert.Equal("Expected", table.Base);
        Assert.Equal(sqlite.Base, table.Base);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndStructuresId()
    {
        const string baseNamespace = "Expected";
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        
        HashAlgorithm algorithm = EntityTest.GetHashAlgorithm();
        
        const string sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Encoding.ASCII.GetString(expectedSqliteHash);
        
        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Encoding.ASCII.GetString(expectedTableHash);
        
        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        
        sqlite.UpdateBase("Test");
        sqlite.AddStructure(table);
        sqlite.UpdateBase("Expected");
        
        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
    }

    [Fact]
    public void AddStructureUpdatesBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        var expected = "Expected";
        sqlite.UpdateBase(expected);
        sqlite.AddStructure(table);
        
        Assert.Equal(expected, sqlite.Base);
        Assert.Equal(sqlite.Base, table.Base);
    }

    [Fact]
    public void AddStructureUpdatesStore()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        
        sqlite.UpdateBase("Test");
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