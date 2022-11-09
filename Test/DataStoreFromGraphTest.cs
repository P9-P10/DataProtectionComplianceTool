using System.Collections.Generic;
using System.Linq;
using GraphManipulation.Extensions;
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

    private static IGraph CreateBaseTestGraph()
    {
        IGraph graph = new Graph();

        graph.NamespaceMap.AddNamespace("ddl", DataStoreToGraph.OntologyNamespace);
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
            .Find<Schema>(_tds.ExpectedSchema1);

        var actualSchema2 = _tds.ActualSqlites.First()
            .Find<Schema>(_tds.ExpectedSchema2);

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
            .Find<Table>(_tds.ExpectedTable1);

        var actualTable2 = _tds.ActualSqlites.First()
            .Find<Table>(_tds.ExpectedTable2);

        Assert.Equal(_tds.ExpectedTable1, actualTable1);
        Assert.Equal(_tds.ExpectedTable2, actualTable2);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumns()
    {
        var actualColumn1 = _tds.ActualSqlites.First()
            .Find<Column>(_tds.ExpectedColumn11);

        var actualColumn2 = _tds.ActualSqlites.First()
            .Find<Column>(_tds.ExpectedColumn12);

        Assert.Equal(_tds.ExpectedColumn11, actualColumn1);
        Assert.Equal(_tds.ExpectedColumn12, actualColumn2);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithSinglePrimaryKey()
    {
        var actual = _tds.ActualSqlites.First()
            .Find<Table>(_tds.ExpectedTable1)!
            .PrimaryKeys;

        Assert.Single(actual);
        Assert.Equal(_tds.ExpectedTable1.PrimaryKeys.First(), actual.First());
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithCompositePrimaryKey()
    {
        var actual = _tds.ActualSqlites.First()
            .Find<Table>(_tds.ExpectedTable3.Uri)!
            .PrimaryKeys;
        
        
        Assert.Equal(3, actual.Count);
        Assert.True(_tds.ExpectedTable3.PrimaryKeys.SequenceEqual(actual));
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithSingleForeignKey()
    {
        var actual = _tds.ActualSqlites.First()
            .Find<Table>(_tds.ExpectedTable1)!
            .ForeignKeys;

        Assert.Single(actual);
        Assert.Equal(_tds.ExpectedTable1.ForeignKeys.First(), actual.First());
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithTablesWithCompositeForeignKey()
    {
        Assert.True(false);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithDataType()
    {
        var actualDataType = _tds.ActualSqlites.First()
            .Find<Column>(_tds.ExpectedColumn11)!
            .DataType;

        Assert.Equal(TestDataStoreFixture.ExpectedColumn11DataType, actualDataType);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithIsNotNull()
    {
        var actualIsNotNull1 = _tds.ActualSqlites.First()
            .Find<Column>(_tds.ExpectedColumn11)!
            .IsNotNull;
        
        var actualIsNotNull2 = _tds.ActualSqlites.First()
            .Find<Column>(_tds.ExpectedColumn12)!
            .IsNotNull;

        Assert.Equal(_tds.ExpectedColumn11.IsNotNull, actualIsNotNull1);
        Assert.Equal(_tds.ExpectedColumn12.IsNotNull, actualIsNotNull2);
    }

    [Fact]
    public void GetDataStoresTypeSqliteReturnsSqliteWithColumnsWithOptions()
    {
        var actualOptions1 = _tds.ActualSqlites.First()
            .Find<Column>(_tds.ExpectedColumn11)!
            .Options;

        var actualOptions2 = _tds.ActualSqlites.First()
            .Find<Column>(_tds.ExpectedColumn12)!
            .Options;

        Assert.Equal(_tds.ExpectedColumn11.Options, actualOptions1);
        Assert.Equal(_tds.ExpectedColumn12.Options, actualOptions2);
    }

    public class TestDataStoreFixture
    {
        public const string ExpectedSqlite1Name = "TestSqlite1";
        public const string ExpectedSqlite2Name = "TestSqlite2";
        public const string ExpectedSchema1Name = "TestSchema1";
        public const string ExpectedSchema2Name = "TestSchema2";
        public const string ExpectedSchema3Name = "TestSchema3";
        public const string ExpectedTable1Name = "TestTable1";
        public const string ExpectedTable2Name = "TestTable2";
        public const string ExpectedTable3Name = "TestTable3";
        public const string ExpectedColumn11Name = "TestColumn11";
        public const string ExpectedColumn12Name = "TestColumn12";
        public const string ExpectedColumn21Name = "TestColumn21";
        public const string ExpectedColumn22Name = "TestColumn22";
        public const string ExpectedColumn23Name = "TestColumn23";
        public const string ExpectedColumn31Name = "TestColumn31";
        public const string ExpectedColumn32Name = "TestColumn32";
        public const string ExpectedColumn33Name = "TestColumn33";

        public const string ExpectedColumn11DataType = "INT";
        public const string ExpectedColumn12DataType = "VARCHAR";
        public const string ExpectedColumn21DataType = "INT";
        public const string ExpectedColumn22DataType = "VARCHAR";
        public const string ExpectedColumn23DataType = "DATETIME";
        public const string ExpectedColumn31DataType = "INT";
        public const string ExpectedColumn32DataType = "VARCHAR";
        public const string ExpectedColumn33DataType = "DATETIME";

        public readonly Sqlite ExpectedSqlite1;
        public readonly Sqlite ExpectedSqlite2;
        public readonly Schema ExpectedSchema1;
        public readonly Schema ExpectedSchema2;
        public readonly Schema ExpectedSchema3;
        public readonly Table ExpectedTable1;
        public readonly Table ExpectedTable2;
        public readonly Table ExpectedTable3;
        
        public readonly Column ExpectedColumn11;
        public readonly Column ExpectedColumn12;
        public readonly Column ExpectedColumn21;
        public readonly Column ExpectedColumn22;
        public readonly Column ExpectedColumn23;
        public readonly Column ExpectedColumn31;
        public readonly Column ExpectedColumn32;
        public readonly Column ExpectedColumn33;

        public List<Sqlite> ActualSqlites;

        public IGraph Graph;

        public TestDataStoreFixture()
        {
            ExpectedSqlite1 = new Sqlite(ExpectedSqlite1Name, BaseUri);
            ExpectedSqlite2 = new Sqlite(ExpectedSqlite2Name, BaseUri);
            ExpectedSchema1 = new Schema(ExpectedSchema1Name);
            ExpectedSchema2 = new Schema(ExpectedSchema2Name);
            ExpectedSchema3 = new Schema(ExpectedSchema3Name);
            ExpectedTable1 = new Table(ExpectedTable1Name);
            ExpectedTable2 = new Table(ExpectedTable2Name);
            ExpectedTable3 = new Table(ExpectedTable3Name);
            ExpectedColumn11 = new Column(ExpectedColumn11Name, ExpectedColumn11DataType, true, "AUTOINCREMENT");
            ExpectedColumn12 = new Column(ExpectedColumn12Name, ExpectedColumn12DataType);
            ExpectedColumn21 = new Column(ExpectedColumn21Name, ExpectedColumn21DataType);
            ExpectedColumn22 = new Column(ExpectedColumn22Name, ExpectedColumn22DataType, true, "AUTOINCREMENT");
            ExpectedColumn23 = new Column(ExpectedColumn23Name, ExpectedColumn23DataType);
            ExpectedColumn31 = new Column(ExpectedColumn31Name, ExpectedColumn31DataType);
            ExpectedColumn32 = new Column(ExpectedColumn32Name, ExpectedColumn32DataType);
            ExpectedColumn33 = new Column(ExpectedColumn33Name, ExpectedColumn33DataType);

            ExpectedSqlite1.AddStructure(ExpectedSchema1);
            ExpectedSqlite1.AddStructure(ExpectedSchema2);
            ExpectedSqlite1.AddStructure(ExpectedSchema3);
            ExpectedSchema1.AddStructure(ExpectedTable1);
            ExpectedSchema1.AddStructure(ExpectedTable2);
            ExpectedSchema3.AddStructure(ExpectedTable3);
            ExpectedTable1.AddStructure(ExpectedColumn11);
            ExpectedTable1.AddStructure(ExpectedColumn12);
            ExpectedTable2.AddStructure(ExpectedColumn21);
            ExpectedTable2.AddStructure(ExpectedColumn22);
            ExpectedTable2.AddStructure(ExpectedColumn23);
            ExpectedTable3.AddStructure(ExpectedColumn31);
            ExpectedTable3.AddStructure(ExpectedColumn32);
            ExpectedTable3.AddStructure(ExpectedColumn33);
            
            ExpectedTable1.AddPrimaryKey(ExpectedColumn11);
            
            ExpectedTable2.AddPrimaryKey(ExpectedColumn21);
            ExpectedTable2.AddPrimaryKey(ExpectedColumn22);
            
            ExpectedTable3.AddPrimaryKey(ExpectedColumn31);
            ExpectedTable3.AddPrimaryKey(ExpectedColumn32);
            ExpectedTable3.AddPrimaryKey(ExpectedColumn33);
            
            ExpectedTable1.AddForeignKey(ExpectedColumn11, ExpectedColumn21);
            
            ExpectedTable2.AddForeignKey(ExpectedColumn21, ExpectedColumn31);
            ExpectedTable2.AddForeignKey(ExpectedColumn22, ExpectedColumn32);
            ExpectedTable2.AddForeignKey(ExpectedColumn23, ExpectedColumn33);

            Graph = CreateBaseTestGraph();

            Graph.AssertNamedEntityTriple(ExpectedSqlite1);
            Graph.AssertNamedEntityTriple(ExpectedSqlite2);
            Graph.AssertNamedEntityTriple(ExpectedSchema1);
            Graph.AssertNamedEntityTriple(ExpectedSchema2);
            Graph.AssertNamedEntityTriple(ExpectedSchema3);
            Graph.AssertNamedEntityTriple(ExpectedTable1);
            Graph.AssertNamedEntityTriple(ExpectedTable2);
            Graph.AssertNamedEntityTriple(ExpectedTable3);
            Graph.AssertNamedEntityTriple(ExpectedColumn11);
            Graph.AssertNamedEntityTriple(ExpectedColumn12);
            Graph.AssertNamedEntityTriple(ExpectedColumn21);
            Graph.AssertNamedEntityTriple(ExpectedColumn22);
            Graph.AssertNamedEntityTriple(ExpectedColumn23);
            Graph.AssertNamedEntityTriple(ExpectedColumn31);
            Graph.AssertNamedEntityTriple(ExpectedColumn32);
            Graph.AssertNamedEntityTriple(ExpectedColumn33);

            Graph.AssertHasStructureTriple(ExpectedSqlite1, ExpectedSchema1);
            Graph.AssertHasStructureTriple(ExpectedSqlite1, ExpectedSchema2);
            Graph.AssertHasStructureTriple(ExpectedSqlite1, ExpectedSchema3);
            Graph.AssertHasStructureTriple(ExpectedSchema1, ExpectedTable1);
            Graph.AssertHasStructureTriple(ExpectedSchema1, ExpectedTable2);
            Graph.AssertHasStructureTriple(ExpectedSchema3, ExpectedTable3);
            Graph.AssertHasStructureTriple(ExpectedTable1, ExpectedColumn11);
            Graph.AssertHasStructureTriple(ExpectedTable1, ExpectedColumn12);
            Graph.AssertHasStructureTriple(ExpectedTable2, ExpectedColumn21);
            Graph.AssertHasStructureTriple(ExpectedTable2, ExpectedColumn22);
            Graph.AssertHasStructureTriple(ExpectedTable2, ExpectedColumn23);
            Graph.AssertHasStructureTriple(ExpectedTable3, ExpectedColumn31);
            Graph.AssertHasStructureTriple(ExpectedTable3, ExpectedColumn32);
            Graph.AssertHasStructureTriple(ExpectedTable3, ExpectedColumn33);
            
            Graph.AssertPrimaryKeys(ExpectedTable1);
            Graph.AssertPrimaryKeys(ExpectedTable2);
            Graph.AssertPrimaryKeys(ExpectedTable3);
            
            Graph.AssertForeignKeys(ExpectedTable1);
            Graph.AssertForeignKeys(ExpectedTable2);
            Graph.AssertForeignKeys(ExpectedTable3);

            Graph.AssertHasDataTypeTriple(ExpectedColumn11);
            Graph.AssertHasDataTypeTriple(ExpectedColumn12);
            Graph.AssertHasDataTypeTriple(ExpectedColumn21);
            Graph.AssertHasDataTypeTriple(ExpectedColumn22);
            Graph.AssertHasDataTypeTriple(ExpectedColumn23);
            Graph.AssertHasDataTypeTriple(ExpectedColumn31);
            Graph.AssertHasDataTypeTriple(ExpectedColumn32);
            Graph.AssertHasDataTypeTriple(ExpectedColumn33);
            
            Graph.AssertIsNotNullTriple(ExpectedColumn11);
            Graph.AssertIsNotNullTriple(ExpectedColumn12);
            Graph.AssertIsNotNullTriple(ExpectedColumn21);
            Graph.AssertIsNotNullTriple(ExpectedColumn22);
            Graph.AssertIsNotNullTriple(ExpectedColumn23);
            Graph.AssertIsNotNullTriple(ExpectedColumn31);
            Graph.AssertIsNotNullTriple(ExpectedColumn32);
            Graph.AssertIsNotNullTriple(ExpectedColumn33);
            
            Graph.AssertOptionsTriple(ExpectedColumn11);
            Graph.AssertOptionsTriple(ExpectedColumn12);
            Graph.AssertOptionsTriple(ExpectedColumn21);
            Graph.AssertOptionsTriple(ExpectedColumn22);
            Graph.AssertOptionsTriple(ExpectedColumn23);
            Graph.AssertOptionsTriple(ExpectedColumn31);
            Graph.AssertOptionsTriple(ExpectedColumn32);
            Graph.AssertOptionsTriple(ExpectedColumn33);

            ActualSqlites = Graph.ConstructDataStores<Sqlite>();
        }
    }

    // TODO: Table Primary og Foreign keys bør nok tilføjes til grafen som en liste, da rækkefølgen af dem er vigtig

    // TODO: Kunne man lave noget i stil med Datastore<Sqlite<Schema<Table<Column>>> ???
    // TODO: Det ville potentielt afskære fra at have flere forskellige typer
    // af Structure (f.eks. Table og Column) i samme niveau. Måske kan man dog lave et system så
    // man kan komponere nye typer nemt, så man f.eks. nemt kan lave en TableAndColumns type  
}