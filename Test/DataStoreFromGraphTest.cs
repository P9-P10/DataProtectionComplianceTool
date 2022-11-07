using System.Linq;
using GraphManipulation.Extensions;
using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using Xunit;

namespace Test;

public class DataStoreFromGraphTest
{
    private const string baseUri = "http://www.test.com/";

    private static IGraph CreateBaseTestGraph()
    {
        IGraph graph = new Graph();

        graph.NamespaceMap.AddNamespace("ddl", GraphBased.OntologyNamespace);
        var uri = UriFactory.Create(baseUri);
        graph.BaseUri = uri;

        return graph;
    }


    [Fact]
    public void GetDataStoresCreatesTheListOfDataStoresOfGivenType()
    {
        var expectedSqlite = new Sqlite("TestSQLite", baseUri);

        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();

        Assert.Equal(expectedSqlite, actualSqlite);
        Assert.Equal(expectedSqlite.Uri, actualSqlite.Uri);
        Assert.Equal(expectedSqlite.BaseUri, actualSqlite.BaseUri);
        Assert.Equal(expectedSqlite.Name, actualSqlite.Name);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithSchemas()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName1 = "TestSchema1";
        const string expectedSchemaName2 = "TestSchema2";

        var expectedSqlite = new Sqlite(expectedSqliteName, baseUri);
        var expectedSchema1 = new Schema(expectedSchemaName1);
        var expectedSchema2 = new Schema(expectedSchemaName2);

        expectedSqlite.AddStructure(expectedSchema1);
        expectedSqlite.AddStructure(expectedSchema2);

        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);
        graph.AssertNamedEntityTriple(expectedSchema1);
        graph.AssertNamedEntityTriple(expectedSchema2);

        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema1);
        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema2);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();

        var actualSchema1 = actualSqlite.FindSchema(expectedSchemaName1);
        var actualSchema2 = actualSqlite.FindSchema(expectedSchemaName2);

        Assert.Equal(expectedSchema1, actualSchema1);
        Assert.Equal(expectedSchema1.Uri, actualSchema1.Uri);
        Assert.Equal(expectedSchema1.Name, actualSchema1.Name);

        Assert.Equal(expectedSchema2, actualSchema2);
        Assert.Equal(expectedSchema2.Uri, actualSchema2.Uri);
        Assert.Equal(expectedSchema2.Name, actualSchema2.Name);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTables()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName = "TestSchema";
        const string expectedTableName1 = "TestTable1";
        const string expectedTableName2 = "TestTable2";

        var expectedSqlite = new Sqlite(expectedSqliteName, baseUri);
        var expectedSchema = new Schema(expectedSchemaName);
        var expectedTable1 = new Table(expectedTableName1);
        var expectedTable2 = new Table(expectedTableName2);

        expectedSqlite.AddStructure(expectedSchema);
        expectedSchema.AddStructure(expectedTable1);
        expectedSchema.AddStructure(expectedTable2);

        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);
        graph.AssertNamedEntityTriple(expectedSchema);
        graph.AssertNamedEntityTriple(expectedTable1);
        graph.AssertNamedEntityTriple(expectedTable2);

        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema);
        graph.AssertHasStructureTriple(expectedSchema, expectedTable1);
        graph.AssertHasStructureTriple(expectedSchema, expectedTable2);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();

        var actualTable1 = actualSqlite
            .FindSchema(expectedSchemaName)
            .FindTable(expectedTableName1);

        var actualTable2 = actualSqlite
            .FindSchema(expectedSchemaName)
            .FindTable(expectedTableName2);

        Assert.Equal(expectedTable1, actualTable1);
        Assert.Equal(expectedTable2, actualTable2);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumns()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName = "TestSchema";
        const string expectedTableName = "TestTable";
        const string expectedColumnName1 = "TestColumn1";
        const string expectedColumnName2 = "TestColumn2";

        var expectedSqlite = new Sqlite(expectedSqliteName, baseUri);
        var expectedSchema = new Schema(expectedSchemaName);
        var expectedTable = new Table(expectedTableName);
        var expectedColumn1 = new Column(expectedColumnName1);
        var expectedColumn2 = new Column(expectedColumnName2);
        
        expectedColumn1.SetDataType("INT");
        expectedColumn2.SetDataType("VARCHAR");

        expectedSqlite.AddStructure(expectedSchema);
        expectedSchema.AddStructure(expectedTable);
        expectedTable.AddStructure(expectedColumn1);
        expectedTable.AddStructure(expectedColumn2);

        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);
        graph.AssertNamedEntityTriple(expectedSchema);
        graph.AssertNamedEntityTriple(expectedTable);
        graph.AssertNamedEntityTriple(expectedColumn1);
        graph.AssertNamedEntityTriple(expectedColumn2);

        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema);
        graph.AssertHasStructureTriple(expectedSchema, expectedTable);
        graph.AssertHasStructureTriple(expectedTable, expectedColumn1);
        graph.AssertHasStructureTriple(expectedTable, expectedColumn2);
        
        graph.AssertHasDataTypeTriple(expectedColumn1);
        graph.AssertHasDataTypeTriple(expectedColumn2);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();

        var actualColumn1 = actualSqlite
            .FindSchema(expectedSchemaName)
            .FindTable(expectedTableName)
            .FindColumn(expectedColumnName1);

        var actualColumn2 = actualSqlite
            .FindSchema(expectedSchemaName)
            .FindTable(expectedTableName)
            .FindColumn(expectedColumnName2);

        Assert.Equal(expectedColumn1, actualColumn1);
        Assert.Equal(expectedColumn2, actualColumn2);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithSinglePrimaryKey()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName = "TestSchema";
        const string expectedTableName = "TestTable";
        const string expectedColumnName = "TestColumn";
        
        
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithCompositePrimaryKey()
    {
        
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithSingleForeignKey()
    {
        
    }
    
    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithCompositeForeignKey()
    {
        
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithDataType()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName = "TestSchema";
        const string expectedTableName = "TestTable";
        const string expectedColumnName = "TestColumn";
        
        var expectedSqlite = new Sqlite(expectedSqliteName, baseUri);
        var expectedSchema = new Schema(expectedSchemaName);
        var expectedTable = new Table(expectedTableName);
        var expectedColumn = new Column(expectedColumnName);
        
        expectedColumn.SetDataType("INT");
        
        expectedSqlite.AddStructure(expectedSchema);
        expectedSchema.AddStructure(expectedTable);
        expectedTable.AddStructure(expectedColumn);
        
        var graph = CreateBaseTestGraph();

        graph.AssertNamedEntityTriple(expectedSqlite);
        graph.AssertNamedEntityTriple(expectedSchema);
        graph.AssertNamedEntityTriple(expectedTable);
        graph.AssertNamedEntityTriple(expectedColumn);

        graph.AssertHasStructureTriple(expectedSqlite, expectedSchema);
        graph.AssertHasStructureTriple(expectedSchema, expectedTable);
        graph.AssertHasStructureTriple(expectedTable, expectedColumn);
        
        graph.AssertHasDataTypeTriple(expectedColumn);

        var actualSqlite = graph.GetDataStores<Sqlite>().First();
        
        var actual = actualSqlite
            .FindSchema(expectedSchemaName)
            .FindTable(expectedTableName)
            .FindColumn(expectedColumnName)
            .DataType;
        
        Assert.Equal(expectedColumn.DataType, actual);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithIsNotNull()
    {
        
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithOptions()
    {
        
    }
    
    // TODO: Table Primary og Foreign keys bør nok tilføjes til grafen som en liste, da rækkefølgen af dem er vigtig
    
    // TODO: Test with multiple SQLites in the same graph

    // TODO: Kunne man lave noget i stil med Datastore<Sqlite<Schema<Table<Column>>> ???
    // TODO: Det ville potentielt afskære fra at have flere forskellige typer
    // af Structure (f.eks. Table og Column) i samme niveau. Måske kan man dog lave et system så
    // man kan komponere nye typer nemt, så man f.eks. nemt kan lave en TableAndColumns type  
}