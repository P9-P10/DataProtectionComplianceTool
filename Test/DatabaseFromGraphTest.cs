using System.Linq;
using GraphManipulation.SchemaEvolution.Extensions;
using GraphManipulation.SchemaEvolution.Models.Stores;
using GraphManipulation.SchemaEvolution.Models.Structures;
using GraphManipulation.SchemaEvolution.Ontologies;
using VDS.RDF;
using Xunit;

namespace Test;

public class DatabaseFromGraphTest : IClassFixture<DatabaseFromGraphTest.TestDatabaseFixture>
{
    private const string BaseUri = "http://www.test.com/";

    private readonly TestDatabaseFixture _tds;

    public DatabaseFromGraphTest(TestDatabaseFixture tds)
    {
        _tds = tds;
    }

    private static IGraph CreateBaseTestGraph()
    {
        IGraph graph = new Graph();

        graph.NamespaceMap.AddNamespace(
            DatabaseDescriptionLanguage.OntologyPrefix,
            DatabaseDescriptionLanguage.OntologyUri);
        var uri = UriFactory.Create(BaseUri);
        graph.BaseUri = uri;

        return graph;
    }


    [Fact]
    public void GetDatabaseCreatesTheDatabaseOfGivenType()
    {
        Assert.Equal(_tds.ExpectedSqlite, _tds.ActualSqlite);
        Assert.Equal(_tds.ExpectedSqlite.Uri, _tds.ActualSqlite.Uri);
        Assert.Equal(_tds.ExpectedSqlite.BaseUri, _tds.ActualSqlite.BaseUri);
        Assert.Equal(_tds.ExpectedSqlite.Name, _tds.ActualSqlite.Name);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithSchemas()
    {
        var actualSchema1 = _tds.ActualSqlite
            .Find<Schema>(_tds.ExpectedSchema1)!;

        var actualSchema2 = _tds.ActualSqlite
            .Find<Schema>(_tds.ExpectedSchema2)!;

        Assert.Equal(_tds.ExpectedSchema1, actualSchema1);
        Assert.Equal(_tds.ExpectedSchema1.Uri, actualSchema1.Uri);
        Assert.Equal(_tds.ExpectedSchema1.Name, actualSchema1.Name);

        Assert.Equal(_tds.ExpectedSchema2, actualSchema2);
        Assert.Equal(_tds.ExpectedSchema2.Uri, actualSchema2.Uri);
        Assert.Equal(_tds.ExpectedSchema2.Name, actualSchema2.Name);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithTables()
    {
        var actualTable1 = _tds.ActualSqlite
            .Find<Table>(_tds.ExpectedTable1);

        var actualTable2 = _tds.ActualSqlite
            .Find<Table>(_tds.ExpectedTable2);

        Assert.Equal(_tds.ExpectedTable1, actualTable1);
        Assert.Equal(_tds.ExpectedTable2, actualTable2);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithColumns()
    {
        var actualColumn1 = _tds.ActualSqlite
            .Find<Column>(_tds.ExpectedColumn11);

        var actualColumn2 = _tds.ActualSqlite
            .Find<Column>(_tds.ExpectedColumn12);

        Assert.Equal(_tds.ExpectedColumn11, actualColumn1);
        Assert.Equal(_tds.ExpectedColumn12, actualColumn2);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithTablesWithSinglePrimaryKey()
    {
        var actual = _tds.ActualSqlite
            .Find<Table>(_tds.ExpectedTable1)!
            .PrimaryKeys;

        Assert.Single(actual);
        Assert.Equal(_tds.ExpectedTable1.PrimaryKeys, actual);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithTablesWithCompositePrimaryKey()
    {
        var actual = _tds.ActualSqlite
            .Find<Table>(_tds.ExpectedTable3.Uri)!
            .PrimaryKeys;


        Assert.Equal(3, actual.Count);
        Assert.True(_tds.ExpectedTable3.PrimaryKeys.SequenceEqual(actual));
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithTablesWithSingleForeignKey()
    {
        var actual = _tds.ActualSqlite
            .Find<Table>(_tds.ExpectedTable1)!
            .ForeignKeys;

        Assert.Single(actual);
        Assert.Equal(_tds.ExpectedTable1.ForeignKeys, actual);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithTablesWithCompositeForeignKey()
    {
        var actual = _tds.ActualSqlite
            .Find<Table>(_tds.ExpectedTable2)!
            .ForeignKeys;

        Assert.Equal(3, actual.Count);
        Assert.True(_tds.ExpectedTable2.ForeignKeys.SequenceEqual(actual));
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithColumnsWithDataType()
    {
        var actualDataType = _tds.ActualSqlite
            .Find<Column>(_tds.ExpectedColumn11)!
            .DataType;

        Assert.Equal(TestDatabaseFixture.ExpectedColumn11DataType, actualDataType);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithColumnsWithIsNotNull()
    {
        var actualIsNotNull1 = _tds.ActualSqlite
            .Find<Column>(_tds.ExpectedColumn11)!
            .IsNotNull;

        var actualIsNotNull2 = _tds.ActualSqlite
            .Find<Column>(_tds.ExpectedColumn12)!
            .IsNotNull;

        Assert.Equal(_tds.ExpectedColumn11.IsNotNull, actualIsNotNull1);
        Assert.Equal(_tds.ExpectedColumn12.IsNotNull, actualIsNotNull2);
    }

    [Fact]
    public void GetDatabaseTypeSqliteReturnsSqliteWithColumnsWithOptions()
    {
        var actualOptions1 = _tds.ActualSqlite
            .Find<Column>(_tds.ExpectedColumn11)!
            .Options;

        var actualOptions2 = _tds.ActualSqlite
            .Find<Column>(_tds.ExpectedColumn12)!
            .Options;

        Assert.Equal(_tds.ExpectedColumn11.Options, actualOptions1);
        Assert.Equal(_tds.ExpectedColumn12.Options, actualOptions2);
    }

    public class TestDatabaseFixture
    {
        private const string ExpectedSqliteName = "TestSqlite";
        private const string ExpectedSchema1Name = "TestSchema1";
        private const string ExpectedSchema2Name = "TestSchema2";
        private const string ExpectedSchema3Name = "TestSchema3";
        private const string ExpectedTable1Name = "TestTable1";
        private const string ExpectedTable2Name = "TestTable2";
        private const string ExpectedTable3Name = "TestTable3";
        private const string ExpectedColumn11Name = "TestColumn11";
        private const string ExpectedColumn12Name = "TestColumn12";
        private const string ExpectedColumn21Name = "TestColumn21";
        private const string ExpectedColumn22Name = "TestColumn22";
        private const string ExpectedColumn23Name = "TestColumn23";
        private const string ExpectedColumn31Name = "TestColumn31";
        private const string ExpectedColumn32Name = "TestColumn32";
        private const string ExpectedColumn33Name = "TestColumn33";

        public const string ExpectedColumn11DataType = "INT";
        public const string ExpectedColumn12DataType = "VARCHAR";
        public const string ExpectedColumn21DataType = "INT";
        public const string ExpectedColumn22DataType = "VARCHAR";
        public const string ExpectedColumn23DataType = "DATETIME";
        public const string ExpectedColumn31DataType = "INT";
        public const string ExpectedColumn32DataType = "VARCHAR";
        public const string ExpectedColumn33DataType = "DATETIME";

        public readonly Column ExpectedColumn11;
        public readonly Column ExpectedColumn12;
        public readonly Column ExpectedColumn21;
        public readonly Column ExpectedColumn22;
        public readonly Column ExpectedColumn23;
        public readonly Column ExpectedColumn31;
        public readonly Column ExpectedColumn32;
        public readonly Column ExpectedColumn33;
        public readonly Schema ExpectedSchema1;
        public readonly Schema ExpectedSchema2;
        public readonly Schema ExpectedSchema3;

        public readonly Sqlite ExpectedSqlite;
        public readonly Table ExpectedTable1;
        public readonly Table ExpectedTable2;
        public readonly Table ExpectedTable3;

        public Sqlite ActualSqlite;

        public IGraph Graph;

        public TestDatabaseFixture()
        {
            ExpectedSqlite = new Sqlite(ExpectedSqliteName, BaseUri);
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

            ExpectedSqlite.AddStructure(ExpectedSchema1);
            ExpectedSqlite.AddStructure(ExpectedSchema2);
            ExpectedSqlite.AddStructure(ExpectedSchema3);
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

            Graph.Merge(ExpectedSqlite.ToGraph());

            ActualSqlite = Graph.ConstructDatabase<Sqlite>()!;
        }
    }
}