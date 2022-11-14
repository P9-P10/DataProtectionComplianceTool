using System;
using GraphManipulation.Extensions;
using GraphManipulation.Manipulation;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using Xunit;

namespace Test;

public class GraphManipulatorTest : IClassFixture<GraphManipulatorTest.TestDataStoreFixture>
{
    private const string BaseUri = "http://www.test.com/";

    private readonly TestDataStoreFixture _tds;

    public GraphManipulatorTest(TestDataStoreFixture tds)
    {
        _tds = tds;
    }

    [Fact]
    public void MoveColumnToOtherParent()
    {
        var graphManipulator = new GraphManipulator<Sqlite>(_tds.ExpectedSqlite.ToGraph());
        
        var columnUriBefore = _tds.ExpectedColumn.Uri.ToString();
        _tds.ExpectedTable2.AddStructure(_tds.ExpectedColumn);
        var columnUriAfter = _tds.ExpectedColumn.Uri.ToString();
        
        graphManipulator.Move(new Uri(columnUriBefore), _tds.ExpectedColumn);
        
        var subj = graphManipulator.Graph.CreateUriNode(_tds.ExpectedTable2.Uri);
        var pred = graphManipulator.Graph.CreateUriNode("ddl:hasStructure");
        var obj = graphManipulator.Graph.CreateUriNode(_tds.ExpectedColumn.Uri);

        Assert.Contains(new Triple(subj, pred, obj), graphManipulator.Graph.Triples);
        
        Assert.Single(graphManipulator.Changes);
        Assert.Contains($"MOVE({columnUriBefore}, {columnUriAfter})", graphManipulator.Changes);
    }

    [Fact]
    public void MoveColumnKeepsAttributesSuchAsColumnType()
    {
        var graphManipulator = new GraphManipulator<Sqlite>(_tds.ExpectedSqlite.ToGraph());

        var columnUriBefore = _tds.ExpectedColumn.Uri.ToString();
        _tds.ExpectedTable2.AddStructure(_tds.ExpectedColumn);
        
        graphManipulator.Move(new Uri(columnUriBefore), _tds.ExpectedColumn);
        
        var subj = graphManipulator.Graph.CreateUriNode(_tds.ExpectedColumn.Uri);
        var pred = graphManipulator.Graph.CreateUriNode("ddl:hasDataType");
        var obj = graphManipulator.Graph.CreateLiteralNode("INT");

        Assert.Contains(new Triple(subj, pred, obj), graphManipulator.Graph.Triples);
    }

    [Fact (Skip = "Future work")]
    public void MoveTableChildrenMoved()
    {
        
    }

    [Fact]
    public void RenameRenamesStructure()
    {
        Assert.True(false);
    }

    [Fact]
    public void RenameAddsChange()
    {
        Assert.True(false);
    }

    [Fact]
    public void RenameAddsChangesForChildrenIdRecomputes()
    {
        Assert.True(false);
    }

    public class TestDataStoreFixture
    {
        private const string ExpectedSqliteName = "TestSqlite";
        private const string ExpectedSchemaName = "TestSchema";
        private const string ExpectedTable1Name = "TestTable1";
        private const string ExpectedTable2Name = "TestTable2";
        private const string ExpectedColumnName = "TestColumn";
        private const string ExpectedPrimaryColumn1Name = "PrimaryColumn1";
        private const string ExpectedPrimaryColumn2Name = "PrimaryColumn2";

        public readonly Sqlite ExpectedSqlite;
        public readonly Schema ExpectedSchema;
        public readonly Table ExpectedTable1;
        public readonly Table ExpectedTable2;
        public readonly Column ExpectedColumn;
        public readonly Column ExpectedPrimaryColumn1;
        public readonly Column ExpectedPrimaryColumn2;

        public TestDataStoreFixture()
        {
            ExpectedSqlite = new Sqlite(ExpectedSqliteName, BaseUri);
            ExpectedSchema = new Schema(ExpectedSchemaName);
            ExpectedTable1 = new Table(ExpectedTable1Name);
            ExpectedTable2 = new Table(ExpectedTable2Name);
            ExpectedColumn = new Column(ExpectedColumnName, "INT");
            ExpectedPrimaryColumn1 = new Column(ExpectedPrimaryColumn1Name);
            ExpectedPrimaryColumn2 = new Column(ExpectedPrimaryColumn2Name);
            
            ExpectedSqlite.AddStructure(ExpectedSchema);
            ExpectedSchema.AddStructure(ExpectedTable1);
            ExpectedSchema.AddStructure(ExpectedTable2);
            ExpectedTable1.AddStructure(ExpectedColumn);
            
            ExpectedTable1.AddStructure(ExpectedPrimaryColumn1);
            ExpectedTable2.AddStructure(ExpectedPrimaryColumn2);
            ExpectedTable1.AddPrimaryKey(ExpectedPrimaryColumn1);
            ExpectedTable2.AddPrimaryKey(ExpectedPrimaryColumn2);
        }
    }
}