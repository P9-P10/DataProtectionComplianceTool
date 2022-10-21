using System.Security.Cryptography;
using System.Text;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class EntityTest
{
    // TODO: Test at Base er det rigtige Uri format
    // TODO: SetBase skal sætte alle børns base også (Både fra en Store og fra en Structure)
    // TODO: SetBase skal sætte alle forældres Base også
    // TODO: UpdateBase skal opdatere base, se om den skal gå op eller ned i strukturen, og så opdatere dem
    
    private HashAlgorithm GetHashAlgorithm()
    {
        return new Column("").Algorithm;
    }

    [Fact]
    public void HashingSanityCheckSameInstance()
    {
        const string testString = "testString";

        var algorithm = GetHashAlgorithm();

        var a = Encoding.ASCII.GetString(algorithm.ComputeHash(Encoding.ASCII.GetBytes(testString)));
        var b = Encoding.ASCII.GetString(algorithm.ComputeHash(Encoding.ASCII.GetBytes(testString)));

        Assert.Equal(a, b);
    }

    [Fact]
    public void HashingSanityCheckDifferentInstances()
    {
        const string testString = "testString";

        var a = Encoding.ASCII.GetString(GetHashAlgorithm().ComputeHash(Encoding.ASCII.GetBytes(testString)));
        var b = Encoding.ASCII.GetString(GetHashAlgorithm().ComputeHash(Encoding.ASCII.GetBytes(testString)));

        Assert.Equal(a, b);
    }

    [Fact]
    public void EntityComparison()
    {
        const string columnName = "Column";

        var columnA = new Column(columnName);
        var columnB = new Column(columnName);

        Assert.Equal(columnA, columnB);
    }

    [Fact]
    public void EntityHashesToExpectedValue()
    {
        const string columnName = "Column";

        var algorithm = GetHashAlgorithm();

        var expectedHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnName));
        var expectedString = Encoding.ASCII.GetString(expectedHash);

        var column = new Column(columnName);

        var actualString = column.Id;

        Assert.Equal(expectedString, actualString);
    }

    [Fact]
    public void EntityCompositionTest()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";
        const string tableName = "Table";
        const string columnName = "Column";

        var algorithm = GetHashAlgorithm();

        const string sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Encoding.ASCII.GetString(expectedSqliteHash);

        const string schemaString = sqliteString + schemaName;
        var expectedSchemaHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(schemaString));
        var expectedSchemaString = Encoding.ASCII.GetString(expectedSchemaHash);

        const string tableString = schemaString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Encoding.ASCII.GetString(expectedTableHash);

        const string columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Encoding.ASCII.GetString(expectedColumnHash);

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

    [Fact]
    public void DataStoreStructureChaining()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";
        const string tableName = "Table";

        var algorithm = GetHashAlgorithm();

        const string sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Encoding.ASCII.GetString(expectedSqliteHash);

        var sqlite = new Sqlite(sqliteName);
        sqlite.SetBase(baseNamespace);

        Assert.Equal(expectedSqliteString, sqlite.Id);

        const string schemaStringBefore = schemaName;
        var expectedSchemaHashBefore = algorithm.ComputeHash(Encoding.ASCII.GetBytes(schemaStringBefore));
        var expectedSchemaStringBefore = Encoding.ASCII.GetString(expectedSchemaHashBefore);

        const string tableStringBefore = schemaStringBefore + tableName;
        var expectedTableHashBefore = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableStringBefore));
        var expectedTableStringBefore = Encoding.ASCII.GetString(expectedTableHashBefore);

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        schema.AddStructure(table);

        Assert.Equal(expectedSchemaStringBefore, schema.Id);
        Assert.Equal(expectedTableStringBefore, table.Id);

        const string schemaStringAfter = sqliteString + schemaName;
        var expectedSchemaHashAfter = algorithm.ComputeHash(Encoding.ASCII.GetBytes(schemaStringAfter));
        var expectedSchemaStringAfter = Encoding.ASCII.GetString(expectedSchemaHashAfter);

        const string tableStringAfter = schemaStringAfter + tableName;
        var expectedTableHashAfter = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableStringAfter));
        var expectedTableStringAfter = Encoding.ASCII.GetString(expectedTableHashAfter);

        sqlite.AddStructure(schema);

        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedSchemaStringAfter, schema.Id);
        Assert.Equal(expectedTableStringAfter, table.Id);
    }

    [Fact]
    public void StructureStructureChaining()
    {
        const string schemaName = "Schema";
        const string tableName = "Table";

        var algorithm = GetHashAlgorithm();

        const string schemaStringBefore = schemaName;
        var expectedSchemaHashBefore = algorithm.ComputeHash(Encoding.ASCII.GetBytes(schemaStringBefore));
        var expectedSchemaStringBefore = Encoding.ASCII.GetString(expectedSchemaHashBefore);

        const string tableStringBefore = tableName;
        var expectedTableHashBefore = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableStringBefore));
        var expectedTableStringBefore = Encoding.ASCII.GetString(expectedTableHashBefore);

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        Assert.Equal(expectedSchemaStringBefore, schema.Id);
        Assert.Equal(expectedTableStringBefore, table.Id);

        const string tableStringAfter = schemaStringBefore + tableName;
        var expectedTableHashAfter = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableStringAfter));
        var expectedTableStringAfter = Encoding.ASCII.GetString(expectedTableHashAfter);

        schema.AddStructure(table);

        Assert.Equal(expectedSchemaStringBefore, schema.Id);
        Assert.Equal(expectedTableStringAfter, table.Id);
    }

    [Fact]
    public void SubStructuresAreUpdatedWhenParentStructureIsAdded()
    {
        const string schemaName = "Schema";
        const string tableName = "Table";
        const string columnName1 = "Column1";
        const string columnName2 = "Column2";

        var algorithm = GetHashAlgorithm();

        const string schemaString = schemaName;
        var expectedSchemaHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(schemaString));
        var expectedSchemaString = Encoding.ASCII.GetString(expectedSchemaHash);

        const string tableStringBefore = tableName;
        var expectedTableHashBefore = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableStringBefore));
        var expectedTableStringBefore = Encoding.ASCII.GetString(expectedTableHashBefore);

        const string columnString1Before = tableStringBefore + columnName1;
        var expectedColumnHash1Before = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString1Before));
        var expectedColumnString1Before = Encoding.ASCII.GetString(expectedColumnHash1Before);

        const string columnString2Before = tableStringBefore + columnName2;
        var expectedColumnHash2Before = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString2Before));
        var expectedColumnString2Before = Encoding.ASCII.GetString(expectedColumnHash2Before);

        var schema = new Schema(schemaName);
        var table = new Table(tableName);
        var column1 = new Column(columnName1);
        var column2 = new Column(columnName2);

        table.AddStructure(column1);
        table.AddStructure(column2);

        Assert.Equal(expectedSchemaString, schema.Id);
        Assert.Equal(expectedTableStringBefore, table.Id);
        Assert.Equal(expectedColumnString1Before, column1.Id);
        Assert.Equal(expectedColumnString2Before, column2.Id);

        const string tableStringAfter = schemaString + tableName;
        var expectedTableHashAfter = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableStringAfter));
        var expectedTableStringAfter = Encoding.ASCII.GetString(expectedTableHashAfter);

        const string columnString1After = tableStringAfter + columnName1;
        var expectedColumnHash1After = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString1After));
        var expectedColumnString1After = Encoding.ASCII.GetString(expectedColumnHash1After);

        const string columnString2After = tableStringAfter + columnName2;
        var expectedColumnHash2After = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString2After));
        var expectedColumnString2After = Encoding.ASCII.GetString(expectedColumnHash2After);

        schema.AddStructure(table);

        Assert.Equal(expectedSchemaString, schema.Id);
        Assert.Equal(expectedTableStringAfter, table.Id);
        Assert.Equal(expectedColumnString1After, column1.Id);
        Assert.Equal(expectedColumnString2After, column2.Id);
    }
}