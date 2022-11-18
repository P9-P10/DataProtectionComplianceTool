using System.Linq;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class EntityTest
{
    private const string baseUri = "http://www.test.com/";

    [Fact]
    public void ListEqualitySanityCheckCorrectOrder()
    {
        var table1 = new Table("Table");
        var table2 = new Table("Table");
        var column11 = new Column("Column1");
        var column12 = new Column("Column2");
        var column21 = new Column("Column1");
        var column22 = new Column("Column2");

        table1.AddStructure(column11);
        table1.AddStructure(column12);
        table2.AddStructure(column21);
        table2.AddStructure(column22);

        Assert.True(table1.SubStructures.SequenceEqual(table2.SubStructures));
    }

    [Fact]
    public void ListEqualitySanityCheckWrongOrder()
    {
        var table1 = new Table("Table");
        var table2 = new Table("Table");
        var column11 = new Column("Column1");
        var column12 = new Column("Column2");
        var column21 = new Column("Column1");
        var column22 = new Column("Column2");

        table1.AddStructure(column11);
        table1.AddStructure(column12);
        table2.AddStructure(column22);
        table2.AddStructure(column21);

        Assert.False(table1.SubStructures.SequenceEqual(table2.SubStructures));
    }

    [Fact]
    public void EntityComparison()
    {
        const string columnName = "Column";

        var columnA = new Column(columnName);
        var columnB = new Column(columnName);

        Assert.Equal(columnA, columnB);
    }

    // TODO: Denne test skal muligvis bare slettes
    [Fact]
    public void EntityIdExpectedValue()
    {
        const string columnName = "Column";

        var column = new Column(columnName);

        var actualString = column.Id;

        Assert.Equal(columnName, actualString);
    }

    [Fact]
    public void EntityCompositionTest()
    {
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";
        const string tableName = "Table";
        const string columnName = "Column";


        var expectedSqliteString = string.Join("", baseUri, sqliteName);
        var expectedSchemaString = string.Join(Entity.IdSeparator, expectedSqliteString, schemaName);
        var expectedTableString = string.Join(Entity.IdSeparator, expectedSchemaString, tableName);
        var expectedColumnString = string.Join(Entity.IdSeparator, expectedTableString, columnName);

        var sqlite = new Sqlite(sqliteName);
        var schema = new Schema(schemaName);
        var table = new Table(tableName);
        var column = new Column(columnName);

        sqlite.UpdateBaseUri(baseUri);
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
        const string sqliteName = "SQLite";
        const string schemaName = "Schema";
        const string tableName = "Table";

        var expectedSqliteString = string.Join("", baseUri, sqliteName);

        var sqlite = new Sqlite(sqliteName);
        sqlite.UpdateBaseUri(baseUri);

        Assert.Equal(expectedSqliteString, sqlite.Id);

        var expectedSchemaStringBefore = schemaName;
        var expectedTableStringBefore = string.Join(Entity.IdSeparator, expectedSchemaStringBefore, tableName);

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        schema.AddStructure(table);

        Assert.Equal(expectedSchemaStringBefore, schema.Id);
        Assert.Equal(expectedTableStringBefore, table.Id);

        var expectedSchemaStringAfter = string.Join(Entity.IdSeparator, expectedSqliteString, schemaName);
        var expectedTableStringAfter = string.Join(Entity.IdSeparator, expectedSchemaStringAfter, tableName);

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

        var expectedSchemaStringBefore = schemaName;
        var expectedTableStringBefore = tableName;

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        Assert.Equal(expectedSchemaStringBefore, schema.Id);
        Assert.Equal(expectedTableStringBefore, table.Id);

        var expectedTableStringAfter = string.Join(Entity.IdSeparator, expectedSchemaStringBefore, tableName);

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

        const string expectedSchemaString = schemaName;

        var expectedTableStringBefore = tableName;
        var expectedColumnString1Before = expectedTableStringBefore + Entity.IdSeparator + columnName1;
        var expectedColumnString2Before = expectedTableStringBefore + Entity.IdSeparator + columnName2;

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

        var expectedTableStringAfter = expectedSchemaString + Entity.IdSeparator + tableName;
        var expectedColumnString1After = expectedTableStringAfter + Entity.IdSeparator + columnName1;
        var expectedColumnString2After = expectedTableStringAfter + Entity.IdSeparator + columnName2;

        schema.AddStructure(table);

        Assert.Equal(expectedSchemaString, schema.Id);
        Assert.Equal(expectedTableStringAfter, table.Id);
        Assert.Equal(expectedColumnString1After, column1.Id);
        Assert.Equal(expectedColumnString2After, column2.Id);
    }
}