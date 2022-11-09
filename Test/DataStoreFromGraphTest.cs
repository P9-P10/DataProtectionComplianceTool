using System;
using System.Collections.Generic;
using System.Linq;
using GraphManipulation.Extensions;
using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using Xunit;

namespace Test;

public class DataStoreFromGraphTest : IClassFixture<DataStoreFromGraphTest.TestDataStoreFixture>
{
    private const string BaseUri = "http://www.test.com/";

    private readonly TestDataStoreFixture _tds;

    public DataStoreFromGraphTest(TestDataStoreFixture tds)
    {
        _tds = tds;
    }

    public class TestDataStoreFixture
    {
        public const string ExpectedSqlite1Name = "TestSqlite1";
        public const string ExpectedSqlite2Name = "TestSqlite2";
        public const string ExpectedSchema1Name = "TestSchema1";
        public const string ExpectedSchema2Name = "TestSchema2";
        public const string ExpectedTable1Name = "TestTable1";
        public const string ExpectedTable2Name = "TestTable2";
        public const string ExpectedColumn1Name = "TestColumn1";
        public const string ExpectedColumn2Name = "TestColumn2";

        public readonly Sqlite ExpectedSqlite1;
        public readonly Sqlite ExpectedSqlite2;
        public readonly Schema ExpectedSchema1;
        public readonly Schema ExpectedSchema2;
        public readonly Table ExpectedTable1;
        public readonly Table ExpectedTable2;
        public readonly Column ExpectedColumn1;
        public readonly Column ExpectedColumn2;

        public const string ExpectedColumn1DataType = "INT";
        public const string ExpectedColumn2DataType = "VARCHAR";

        public IGraph Graph;

        public List<Sqlite> ActualSqlites;
        
        public TestDataStoreFixture()
        {
            ExpectedSqlite1 = new Sqlite(ExpectedSqlite1Name, BaseUri);
            ExpectedSqlite2 = new Sqlite(ExpectedSqlite2Name, BaseUri);
            ExpectedSchema1 = new Schema(ExpectedSchema1Name);
            ExpectedSchema2 = new Schema(ExpectedSchema2Name);
            ExpectedTable1 = new Table(ExpectedTable1Name);
            ExpectedTable2 = new Table(ExpectedTable2Name);
            ExpectedColumn1 = new Column(ExpectedColumn1Name);
            ExpectedColumn2 = new Column(ExpectedColumn2Name);
            
            ExpectedColumn1.SetDataType(ExpectedColumn1DataType);
            ExpectedColumn2.SetDataType(ExpectedColumn2DataType);
            
            ExpectedSqlite1.AddStructure(ExpectedSchema1);
            ExpectedSqlite1.AddStructure(ExpectedSchema2);
            ExpectedSchema1.AddStructure(ExpectedTable1);
            ExpectedSchema1.AddStructure(ExpectedTable2);
            ExpectedTable1.AddStructure(ExpectedColumn1);
            ExpectedTable1.AddStructure(ExpectedColumn2);

            Graph = CreateBaseTestGraph();
            
            Graph.AssertNamedEntityTriple(ExpectedSqlite1);
            Graph.AssertNamedEntityTriple(ExpectedSqlite2);
            Graph.AssertNamedEntityTriple(ExpectedSchema1);
            Graph.AssertNamedEntityTriple(ExpectedSchema2);
            Graph.AssertNamedEntityTriple(ExpectedTable1);
            Graph.AssertNamedEntityTriple(ExpectedTable2);
            Graph.AssertNamedEntityTriple(ExpectedColumn1);
            Graph.AssertNamedEntityTriple(ExpectedColumn2);
            
            Graph.AssertHasStructureTriple(ExpectedSqlite1, ExpectedSchema1);
            Graph.AssertHasStructureTriple(ExpectedSqlite1, ExpectedSchema2);
            Graph.AssertHasStructureTriple(ExpectedSchema1, ExpectedTable1);
            Graph.AssertHasStructureTriple(ExpectedSchema1, ExpectedTable2);
            Graph.AssertHasStructureTriple(ExpectedTable1, ExpectedColumn1);
            Graph.AssertHasStructureTriple(ExpectedTable1, ExpectedColumn2);
            
            Graph.AssertHasDataTypeTriple(ExpectedColumn1);
            Graph.AssertHasDataTypeTriple(ExpectedColumn2);

            ActualSqlites = Graph.GetDataStores<Sqlite>();
        }
    }

    private static IGraph CreateBaseTestGraph()
    {
        IGraph graph = new Graph();

        graph.NamespaceMap.AddNamespace("ddl", GraphBased.OntologyNamespace);
        var uri = UriFactory.Create(BaseUri);
        graph.BaseUri = uri;

        return graph;
    }


    [Fact]
    public void GetDataStoresCreatesTheListOfDataStoresOfGivenType()
    {
        Assert.Equal(_tds.ExpectedSqlite1, _tds.ActualSqlites.First());
        Assert.Equal(_tds.ExpectedSqlite1.Uri, _tds.ActualSqlites.First().Uri);
        Assert.Equal(_tds.ExpectedSqlite1.BaseUri, _tds.ActualSqlites.First().BaseUri);
        Assert.Equal(_tds.ExpectedSqlite1.Name, _tds.ActualSqlites.First().Name);
        
        Assert.Equal(_tds.ExpectedSqlite2, _tds.ActualSqlites.Skip(1).First());
        Assert.Equal(_tds.ExpectedSqlite2.Uri, _tds.ActualSqlites.Skip(1).First().Uri);
        Assert.Equal(_tds.ExpectedSqlite2.BaseUri, _tds.ActualSqlites.Skip(1).First().BaseUri);
        Assert.Equal(_tds.ExpectedSqlite2.Name, _tds.ActualSqlites.Skip(1).First().Name);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithSchemas()
    {
        var actualSchema1 = _tds.ActualSqlites.First()
            .FindSchema(TestDataStoreFixture.ExpectedSchema1Name);
        
        var actualSchema2 = _tds.ActualSqlites.First()
            .FindSchema(TestDataStoreFixture.ExpectedSchema2Name);

        Assert.Equal(_tds.ExpectedSchema1, actualSchema1);
        Assert.Equal(_tds.ExpectedSchema1.Uri, actualSchema1.Uri);
        Assert.Equal(_tds.ExpectedSchema1.Name, actualSchema1.Name);

        Assert.Equal(_tds.ExpectedSchema2, actualSchema2);
        Assert.Equal(_tds.ExpectedSchema2.Uri, actualSchema2.Uri);
        Assert.Equal(_tds.ExpectedSchema2.Name, actualSchema2.Name);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTables()
    {
        var actualTable1 = _tds.ActualSqlites.First()
            .FindSchema(TestDataStoreFixture.ExpectedSchema1Name)
            .FindTable(TestDataStoreFixture.ExpectedTable1Name);

        var actualTable2 = _tds.ActualSqlites.First()
            .FindSchema(TestDataStoreFixture.ExpectedSchema1Name)
            .FindTable(TestDataStoreFixture.ExpectedTable2Name);

        Assert.Equal(_tds.ExpectedTable1, actualTable1);
        Assert.Equal(_tds.ExpectedTable2, actualTable2);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumns()
    {
        var actualColumn1 = _tds.ActualSqlites.First()
            .FindSchema(TestDataStoreFixture.ExpectedSchema1Name)
            .FindTable(TestDataStoreFixture.ExpectedTable1Name)
            .FindColumn(TestDataStoreFixture.ExpectedColumn1Name);

        var actualColumn2 = _tds.ActualSqlites.First()
            .FindSchema(TestDataStoreFixture.ExpectedSchema1Name)
            .FindTable(TestDataStoreFixture.ExpectedTable1Name)
            .FindColumn(TestDataStoreFixture.ExpectedColumn2Name);

        Assert.Equal(_tds.ExpectedColumn1, actualColumn1);
        Assert.Equal(_tds.ExpectedColumn2, actualColumn2);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithSinglePrimaryKey()
    {
        const string expectedSqliteName = "TestSQLite";
        const string expectedSchemaName = "TestSchema";
        const string expectedTableName = "TestTable";
        const string expectedColumnName = "TestColumn";
        
        Assert.True(false);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithCompositePrimaryKey()
    {
        Assert.True(false);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithSingleForeignKey()
    {
        Assert.True(false);
    }
    
    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithCompositeForeignKey()
    {
        Assert.True(false);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithDataType()
    {
        var actual = _tds.ActualSqlites.First()
            .FindSchema(TestDataStoreFixture.ExpectedSchema1Name)
            .FindTable(TestDataStoreFixture.ExpectedTable1Name)
            .FindColumn(TestDataStoreFixture.ExpectedColumn1Name)
            .DataType;
        
        Assert.Equal(TestDataStoreFixture.ExpectedColumn1DataType, actual);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithIsNotNull()
    {
        Assert.True(false);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithOptions()
    {
        Assert.True(false);
    }
    
    // TODO: Table Primary og Foreign keys bør nok tilføjes til grafen som en liste, da rækkefølgen af dem er vigtig

    // TODO: Kunne man lave noget i stil med Datastore<Sqlite<Schema<Table<Column>>> ???
    // TODO: Det ville potentielt afskære fra at have flere forskellige typer
    // af Structure (f.eks. Table og Column) i samme niveau. Måske kan man dog lave et system så
    // man kan komponere nye typer nemt, så man f.eks. nemt kan lave en TableAndColumns type  
}