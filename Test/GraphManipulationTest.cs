using System.Security.Cryptography;
using System.Text;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class GraphManipulationTest
{
    public HashAlgorithm GetHashAlgorithm()
    {
        return SHA256.Create();
    }

    [Fact]
    public void HashingSanityCheckSameInstance()
    {
        var testString = "testString";

        var algorithm = GetHashAlgorithm();

        var a = Encoding.UTF8.GetString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(testString)));
        var b = Encoding.UTF8.GetString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(testString)));

        Assert.Equal(a, b);
    }

    [Fact]
    public void HashingSanityCheckDifferentInstances()
    {
        var testString = "testString";

        var a = Encoding.UTF8.GetString(GetHashAlgorithm().ComputeHash(Encoding.UTF8.GetBytes(testString)));
        var b = Encoding.UTF8.GetString(GetHashAlgorithm().ComputeHash(Encoding.UTF8.GetBytes(testString)));

        Assert.Equal(a, b);
    }

    [Fact]
    public void EntityComparison()
    {
        var columnName = "Column";

        var columnA = new Column(columnName);
        var columnB = new Column(columnName);

        Assert.Equal(columnA, columnB);
    }

    [Fact]
    public void ColumnHashesToExpectedValue()
    {
        var columnName = "Column";

        var algorithm = GetHashAlgorithm();

        var expectedHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(columnName));
        var expectedString = Encoding.UTF8.GetString(expectedHash);

        var column = new Column(columnName);

        var actualString = column.Id;

        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void StructureCompositionTest()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";
        const string tableName = "Table";
        const string columnName = "Column";

        var algorithm = GetHashAlgorithm();

        var sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(sqliteString));
        var expectedSqliteString = Encoding.UTF8.GetString(expectedSqliteHash);

        var schemaString = sqliteString + schemaName;
        var expectedSchemaHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(schemaString));
        var expectedSchemaString = Encoding.UTF8.GetString(expectedSchemaHash);

        var tableString = schemaString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(tableString));
        var expectedTableString = Encoding.UTF8.GetString(expectedTableHash);

        var columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(columnString));
        var expectedColumnString = Encoding.UTF8.GetString(expectedColumnHash);

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);
        var table = new Table(tableName);
        var column = new Column(columnName);

        sqlite.SetBase(baseNamespace);
        sqlite.AddStructure(schema);
        schema.AddStructure(table);
        table.AddStructure(column);

        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedSchemaString, schema.Id);
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumnString, column.Id);
    }

    // TODO: Test ID på en store og en kæde af structures hver for sig, og tjek så at det er rigtigt efter de er sat sammen
    // TODO: Test at en structure kan eksistere og have den rigtige ID uden en store
    // TODO: Test at en store kan eksistere og have den rigtige ID uden structure
    // TODO: Der skal nok også opdateres ID når SetStore bliver kaldt
}