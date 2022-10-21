using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class StructureTest
{
    [Fact]
    public void StructuresWithSameNameUniqueUnderDifferentParentStructures()
    {
        const string tableName1 = "Table1";
        const string tableName2 = "Table2";
        const string columnName = "Column";

        var table1 = new Table(tableName1);
        var table2 = new Table(tableName2);
        var column1 = new Column(columnName);
        var column2 = new Column(columnName);

        Assert.Equal(column1, column2);

        table1.AddStructure(column1);
        table2.AddStructure(column2);

        Assert.NotEqual(column1, column2);
    }

    [Fact]
    public void SetStoreSetsStore()
    {
        var sqlite = new Sqlite("SQLite");
        var column = new Column("Column");

        sqlite.SetBase("Test");
        column.AddToStore(sqlite);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void SetStoreAddsStructureToStoreListOfStructures()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.SetBase("Test");
        table.AddToStore(sqlite);

        Assert.Contains(table, sqlite.Structures);
    }

    [Fact]
    public void AddStructureAddsGivenStructureToListOfStructures()
    {
        const string schemaName = "Schema";
        const string tableName = "Table";

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        schema.AddStructure(table);
        Assert.Contains(table, schema.SubStructures);
    }

    [Fact]
    public void StructureCannotBeAddedMultipleTimesToStructureListOfSubStructures()
    {
        const string schemaName = "Schema";
        const string tableName = "Table";

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        schema.AddStructure(table);
        schema.AddStructure(table);
        Assert.Single(schema.SubStructures);
    }
}