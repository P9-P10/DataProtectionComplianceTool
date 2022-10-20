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
        string testString = "testString";

        HashAlgorithm algorithm = GetHashAlgorithm();

        string a = Encoding.UTF8.GetString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(testString)));
        string b = Encoding.UTF8.GetString(algorithm.ComputeHash(Encoding.UTF8.GetBytes(testString)));
        
        Assert.Equal(a, b);
    }

    [Fact]
    public void HashingSanityCheckDifferentInstances()
    {
        string testString = "testString";

        string a = Encoding.UTF8.GetString(GetHashAlgorithm().ComputeHash(Encoding.UTF8.GetBytes(testString)));
        string b = Encoding.UTF8.GetString(GetHashAlgorithm().ComputeHash(Encoding.UTF8.GetBytes(testString)));
        
        Assert.Equal(a, b);
    }

    [Fact]
    public void EntityComparison()
    {
        string columnName = "Column";
        
        Column columnA = new Column(columnName);
        Column columnB = new Column(columnName);

        Assert.Equal(columnA, columnB);
    }

    [Fact]
    public void ColumnHashesToExpectedValue()
    {
        string columnName = "Column";

        HashAlgorithm algorithm = GetHashAlgorithm();
        
        byte[] expectedHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(columnName));
        string expectedString = Encoding.UTF8.GetString(expectedHash);
        
        Column column = new Column(columnName);

        string actualString = column.Id;
        
        Assert.Equal(expectedString, actualString);
    }
    
    [Fact]
    public void StructureCompositionTest()
    {
        string baseNamespace = "Test";
        string sqliteName = "SQLite";
        string schemaName = "Schema";
        string tableName = "Table";
        string columnName = "Column";
        string concatenatedString = baseNamespace + sqliteName + schemaName + tableName + columnName;

        HashAlgorithm algorithm = GetHashAlgorithm();
        
        byte[] expectedHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
        string expectedString = Encoding.UTF8.GetString(expectedHash);

        // byte[] baseHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(baseNamespace));
        // string baseString = Encoding.UTF8.GetString(baseHash);

        Sqlite sqlite = new Sqlite(sqliteName);
        Schema schema = new Schema(schemaName);
        Table table = new Table(tableName);
        Column column = new Column(columnName);
        
        sqlite.SetBase(baseNamespace);
        sqlite.AddStructure(schema);
        schema.AddStructure(table);
        table.AddStructure(column);
        
        // TODO: Remove this statement
        column.ComputeId();

        string actualString = column.Id;
        
        Assert.Equal(expectedString, actualString);
    }
}