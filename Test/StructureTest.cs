using System.Security.Cryptography;
using System.Text;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
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
    public void UpdateStoreSetsStore()
    {
        var sqlite = new Sqlite("SQLite");
        var column = new Column("Column");

        sqlite.UpdateBase("Test");
        column.UpdateStore(sqlite);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void UpdateStoreDoesNotAddStructureToStoreListOfStructures()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.UpdateBase("Test");
        table.UpdateStore(sqlite);
        
        Assert.DoesNotContain(table, sqlite.Structures);
    }

    [Fact]
    public void UpdateStoreUpdatesParentStore()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column = new Column("Column");
        
        sqlite.UpdateBase("Test");
        
        table.AddStructure(column);
        column.UpdateStore(sqlite);
        
        Assert.Equal(sqlite, table.Store);
    }

    [Fact]
    public void UpdateStoreUpdatesSubStructuresStore()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");
        
        sqlite.UpdateBase("Test");
        
        table.AddStructure(column1);
        table.AddStructure(column2);
        
        table.UpdateStore(sqlite);
        
        Assert.Equal(sqlite, column1.Store);
        Assert.Equal(sqlite, column2.Store);
    }

    [Fact]
    public void UpdateStoreUpdatesOwnAndParentId()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName = "Column";
        
        HashAlgorithm algorithm = EntityTest.GetHashAlgorithm();
        
        const string sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Encoding.ASCII.GetString(expectedSqliteHash);
        
        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Encoding.ASCII.GetString(expectedTableHash);

        const string columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Encoding.ASCII.GetString(expectedColumnHash);
        
        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column = new Column(columnName);
        
        sqlite.UpdateBase(baseNamespace);
        
        table.AddStructure(column);
        column.UpdateStore(sqlite);
        
        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumnString, column.Id);
    }
    
    [Fact]
    public void UpdateStoreUpdatesOwnAndSubStructuresId()
    {
        const string baseNamespace = "Test";
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName1 = "Column1";
        const string columnName2 = "Column2";
        
        HashAlgorithm algorithm = EntityTest.GetHashAlgorithm();
        
        const string sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Encoding.ASCII.GetString(expectedSqliteHash);
        
        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Encoding.ASCII.GetString(expectedTableHash);

        const string columnString1 = tableString + columnName1;
        var expectedColumn1Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString1));
        var expectedColumn1String = Encoding.ASCII.GetString(expectedColumn1Hash);
        
        const string columnString2 = tableString + columnName2;
        var expectedColumn2Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString2));
        var expectedColumn2String = Encoding.ASCII.GetString(expectedColumn2Hash);
        
        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column1 = new Column(columnName1);
        var column2 = new Column(columnName2);
        
        sqlite.UpdateBase("Test");
        
        table.AddStructure(column1);
        table.AddStructure(column2);
        
        table.UpdateStore(sqlite);
        
        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumn1String, column1.Id);
        Assert.Equal(expectedColumn2String, column2.Id);
    }

    [Fact]
    public void UpdateBaseUpdatesParentsBase()
    {
        var table = new Table("Table");
        var column = new Column("Column");
        
        table.AddStructure(column);
        column.UpdateBase("Expected");
        
        Assert.Equal("Expected", column.Base);
        Assert.Equal(column.Base, table.Base);
    }

    [Fact]
    public void UpdateBaseUpdatesStoreBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column = new Column("Column");
        
        sqlite.UpdateBase("Test");
        sqlite.AddStructure(table);
        table.AddStructure(column);
        column.UpdateBase("Expected");
        
        Assert.Equal("Expected", sqlite.Base);
    }

    [Fact]
    public void UpdateBaseUpdatesSubStructuresBase()
    {
        var table = new Table("Table");
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");
        
        table.AddStructure(column1);
        table.AddStructure(column2);
        
        table.UpdateBase("Expected");
        
        Assert.Equal("Expected", table.Base);
        Assert.Equal(table.Base, column1.Base);
        Assert.Equal(table.Base, column2.Base);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndParentId()
    {
        const string baseNamespace = "Expected";
        const string tableName = "Table";
        const string columnName = "Column";
        
        HashAlgorithm algorithm = EntityTest.GetHashAlgorithm();
        
        const string tableString = baseNamespace + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Encoding.ASCII.GetString(expectedTableHash);

        const string columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Encoding.ASCII.GetString(expectedColumnHash);
        
        var table = new Table(tableName);
        var column = new Column(columnName);

        table.AddStructure(column);
        column.UpdateBase(baseNamespace);
        
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumnString, column.Id);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndStoreId()
    {
        const string baseNamespace = "Expected";
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName = "Column";
        
        HashAlgorithm algorithm = EntityTest.GetHashAlgorithm();
        
        const string sqliteString = baseNamespace + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Encoding.ASCII.GetString(expectedSqliteHash);
        
        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Encoding.ASCII.GetString(expectedTableHash);

        const string columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Encoding.ASCII.GetString(expectedColumnHash);
        
        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column = new Column(columnName);
        
        sqlite.UpdateBase("Test");
        
        sqlite.AddStructure(table);
        table.AddStructure(column);
        column.UpdateBase(baseNamespace);
        
        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumnString, column.Id);
    }

    [Fact]
    public void UpdateBaseUpdatesSubStructuresId()
    {
        const string baseNamespace = "Expected";
        const string tableName = "Table";
        const string columnName1 = "Column1";
        const string columnName2 = "Column2";
        
        HashAlgorithm algorithm = EntityTest.GetHashAlgorithm();

        const string tableString = baseNamespace + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Encoding.ASCII.GetString(expectedTableHash);

        const string columnString1 = tableString + columnName1;
        var expectedColumn1Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString1));
        var expectedColumn1String = Encoding.ASCII.GetString(expectedColumn1Hash);
        
        const string columnString2 = tableString + columnName2;
        var expectedColumn2Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString2));
        var expectedColumn2String = Encoding.ASCII.GetString(expectedColumn2Hash);
        
        var table = new Table(tableName);
        var column1 = new Column(columnName1);
        var column2 = new Column(columnName2);
        
        table.AddStructure(column1);
        table.AddStructure(column2);
        
        table.UpdateBase(baseNamespace);
        
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumn1String, column1.Id);
        Assert.Equal(expectedColumn2String, column2.Id);
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

    [Fact]
    public void AddStructureSetsSubStructureStore()
    {
        const string baseURI = "http://www.test.com/";
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName = "Column";

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column = new Column(columnName);
        
        sqlite.UpdateBase(baseURI);
        sqlite.AddStructure(table);
        
        table.AddStructure(column);
        
        Assert.Equal(sqlite, table.Store);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void AddStructureUpdatesBase()
    {
        const string baseName = "Expected";
        var table = new Table("Table");
        var column = new Column("Column");
        
        table.UpdateBase(baseName);
        table.AddStructure(column);
        
        Assert.Equal(baseName, table.Base);
        Assert.Equal(table.Base, column.Base);
    }

    [Fact]
    public void AddStructureUpdatesStore()
    {
        const string baseName = "Expected";
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column = new Column("Column");
        
        sqlite.UpdateBase(baseName);
        sqlite.AddStructure(table);
        table.AddStructure(column);
        
        Assert.Equal(sqlite, table.Store);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void StructureWithBaseIdWithBase()
    {
        const string baseNamespace = "Expected";
        const string columnName = "Column";
        
        HashAlgorithm algorithm = EntityTest.GetHashAlgorithm();
        
        const string columnString = baseNamespace + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Encoding.ASCII.GetString(expectedColumnHash);
        
        var column = new Column("Column");
        column.UpdateBase(baseNamespace);
        
        Assert.Equal(expectedColumnString, column.Id);
        Assert.Equal("ExpectedColumn", column.HashedFrom);
    }
    
    public class ToGraphTest
    {
        // TODO: Test at Structures børn bliver lavet til grafer og tilføjet
        [Fact]
        public void BasicTest()
        {
            const string baseName = "http://www.test.com/";
            const string columnName = "TestColumn";
            var column = new Column(columnName);
            column.UpdateBase(baseName);
            
            var graph = column.ToGraph();
            
            Assert.True(graph.NamespaceMap.HasNamespace("rdf"));
            Assert.True(graph.NamespaceMap.HasNamespace("ddl"));
            
            var subj = graph.CreateUriNode(UriFactory.Create(column.Base + column.Id));
            var pred = graph.CreateUriNode("rdf:type");
            var obj = graph.CreateUriNode("ddl:Column");

            Assert.True(graph.ContainsTriple(new Triple(subj, pred, obj)));

            var pred2 = graph.CreateUriNode("ddl:hasName");
            var obj2 = graph.CreateLiteralNode(columnName);
            
            Assert.True(graph.ContainsTriple(new Triple(subj, pred2, obj2)));
        }
    }

    public class FromGraphTest
    {
        
    }
}