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
    public void SetStoreSetsStore()
    {
        var sqlite = new Sqlite("SQLite");
        var column = new Column("Column");

        sqlite.SetBase("Test");
        column.SetStore(sqlite);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void SetStoreAddsStructureToStoreListOfStructures()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.SetBase("Test");
        table.SetStore(sqlite);

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
        
        sqlite.SetBase(baseURI);
        sqlite.AddStructure(table);
        
        table.AddStructure(column);
        
        Assert.Equal(sqlite, table.Store);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void AddToStoreSetsParentStructureStore()
    {
        
    }
    
    // TODO: SetStore skal sætte alle børns Store også
    // TODO: SetStore skal sætte alle forældre Store også
    // TODO: UpdateStore skal opdatere store, se om den skal gå op eller ned i strukturen, og så opdatere dem også
    
    
    // TODO: AddStructure opdaterer Base og Store

    // TODO: Hvis en Structure har en Base, vil dens Id være en kombination af Base og Name. Måske ikke?
    
    
    public class ToGraphTest
    {
        // TODO: Test at Structures børn bliver lavet til grafer og tilføjet
        [Fact]
        public void BasicTest()
        {
            const string baseName = "http://www.test.com/";
            const string columnName = "TestColumn";
            var column = new Column(columnName);
            column.SetBase(baseName);
            
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