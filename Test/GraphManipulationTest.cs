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
        const string concatenatedString = baseNamespace + sqliteName + schemaName + tableName + columnName;

        var algorithm = GetHashAlgorithm();
        
        // var expectedSqliteHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
        // var expectedSqliteString = Encoding.UTF8.GetString(expectedSqliteHash);
        //
        // var expectedSchemaHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
        // var expectedSchemaString = Encoding.UTF8.GetString(expectedSchemaHash);
        //
        // var expectedTableHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
        // var expectedTableString = Encoding.UTF8.GetString(expectedTableHash);

        var expectedColumnHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
        var expectedColumnString = Encoding.UTF8.GetString(expectedColumnHash);

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);
        var table = new Table(tableName);
        var column = new Column(columnName);

        sqlite.SetBase(baseNamespace);
        sqlite.AddStructure(schema);
        schema.AddStructure(table);
        table.AddStructure(column);

        var actualString = column.Id;

        Assert.Equal(expectedColumnString, actualString);
    }

    // TODO: Test ID på en store og en kæde af structures før de sættes sammen, og tjek så at det er rigtigt efter de er sat sammen
    // TODO: Test at en structure kan eksistere og have den rigtige ID uden en store
}