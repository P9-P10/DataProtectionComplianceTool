using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using Xunit;

namespace Test;

public class GraphBasedTest
{
    private const string baseUri = "http://www.test.com/";

    public class ToGraph
    {
        [Fact]
        public void ResultingGraphHasBaseUri()
        {
            var column = new Column("Column");
            column.UpdateBaseUri(baseUri);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            Assert.Equal(UriFactory.Create(baseUri), graph.BaseUri);
        }

        [Fact]
        public void ResultingGraphHasOntologyNamespace()
        {
            var column = new Column("Column");
            column.UpdateBaseUri(baseUri);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            Assert.True(graph.NamespaceMap.HasNamespace("rdf"));
            Assert.True(graph.NamespaceMap.HasNamespace("ddl"));
        }

        [Fact]
        public void EntityWithoutBaseThrowsException()
        {
            var column = new Column("Column");

            Assert.Throws<EntityException>(() => column.ToGraph());
        }

        [Fact]
        public void NamedEntityReturnsGraphWithName()
        {
            const string columnName = "MyColumn";
            var column = new Column(columnName);
            column.UpdateBaseUri(baseUri);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(column.Uri);
            var pred = graph.CreateUriNode("ddl:hasName");
            var obj = graph.CreateLiteralNode(columnName);

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }

        [Fact]
        public void EntityReturnGraphWithType()
        {
            const string columnName = "MyColumn";
            var column = new Column(columnName);
            column.UpdateBaseUri(baseUri);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(column.Uri);
            var pred = graph.CreateUriNode("rdf:type");
            var obj = graph.CreateUriNode("ddl:Column");

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }

        [Fact]
        public void StructuredEntityReturnsGraphWithHasStructure()
        {
            var parent = new Column("Parent");
            var child = new Column("Child");

            parent.UpdateBaseUri(baseUri);
            parent.AddStructure(child);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(parent);

            var graph = parent.ToGraph();

            var triple = new Triple(
                graph.CreateUriNode(parent.Uri),
                graph.CreateUriNode("ddl:hasStructure"),
                graph.CreateUriNode(child.Uri)
            );

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void StoreSubStructuresAddedToGraph()
        {
            var sqlite = new Sqlite("SQLite");
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");

            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(table1);
            sqlite.AddStructure(table2);

            // To avoid exceptions because of no primary key:
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");
            table1.AddStructure(column1);
            table2.AddStructure(column2);
            table1.AddPrimaryKey(column1);
            table2.AddPrimaryKey(column2);

            var graph = sqlite.ToGraph();

            var triple1 = new Triple(
                graph.CreateUriNode(sqlite.Uri),
                graph.CreateUriNode("ddl:hasStructure"),
                graph.CreateUriNode(table1.Uri)
            );

            var triple2 = new Triple(
                graph.CreateUriNode(sqlite.Uri),
                graph.CreateUriNode("ddl:hasStructure"),
                graph.CreateUriNode(table2.Uri)
            );

            Assert.Contains(triple1, graph.Triples);
            Assert.Contains(triple2, graph.Triples);
        }

        [Fact]
        public void StructureSubStructuresAddedToGraph()
        {
            var schema = new Schema("Schema");
            var table = new Table("Table");
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");

            schema.UpdateBaseUri(baseUri);
            schema.AddStructure(table);
            table.AddStructure(column1);
            table.AddStructure(column2);
            table.AddPrimaryKey(column1);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(schema);

            var graph = schema.ToGraph();

            var triple1 = new Triple(
                graph.CreateUriNode(schema.Uri),
                graph.CreateUriNode("ddl:hasStructure"),
                graph.CreateUriNode(table.Uri)
            );

            var triple2 = new Triple(
                graph.CreateUriNode(table.Uri),
                graph.CreateUriNode("ddl:hasStructure"),
                graph.CreateUriNode(column1.Uri)
            );

            var triple3 = new Triple(
                graph.CreateUriNode(table.Uri),
                graph.CreateUriNode("ddl:hasStructure"),
                graph.CreateUriNode(column2.Uri)
            );

            Assert.Contains(triple1, graph.Triples);
            Assert.Contains(triple2, graph.Triples);
            Assert.Contains(triple3, graph.Triples);
        }

        [Fact]
        public void StructureHasStoreAddedToGraph()
        {
            var sqlite = new Sqlite("SQLite");
            var column = new Column("Column");

            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = sqlite.ToGraph();

            var triple = new Triple(
                graph.CreateUriNode(column.Uri),
                graph.CreateUriNode("ddl:hasStore"),
                graph.CreateUriNode(sqlite.Uri)
            );

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void StructureHasNoStoreWhenBuildingGraphThrowsException()
        {
            var column = new Column("Column");

            column.UpdateBaseUri(baseUri);

            Assert.Throws<StructureException>(() => column.ToGraph());
        }
    }

    public class FromGraph
    {
        // [Fact]
        // public void ColumnFromGraph()
        // {
        //     // var sqlite = new Sqlite("SQLite", baseUri);
        //     // var schema = new Schema("Schema");
        //     // var table = new Table("Table");
        //     // var column1 = new Column("Column1", "INT", true);
        //     // var column2 = new Column("Column2", "VARCHAR(255)", false, "AUTOINCREMENT");
        //     //
        //     // sqlite.AddStructure(schema);
        //     // schema.AddStructure(table);
        //     // table.AddStructure(column1);
        //     // table.AddStructure(column2);
        //     // table.AddPrimaryKey(column1);
        //
        //     var expected = new Column("Column", "INT");
        //     expected.UpdateBaseUri(baseUri);
        //     
        //     IGraph graph = new Graph();
        //     
        //     graph.NamespaceMap.AddNamespace("ddl", GraphBased.OntologyNamespace);
        //     var uri = UriFactory.Create(baseUri);
        //     graph.BaseUri = uri;
        //
        //     var subj = graph.CreateUriNode(expected.Uri);
        //     var pred = graph.CreateUriNode("rdf:type");
        //     var obj = graph.CreateUriNode("ddl:Column");
        //
        //     graph.Assert(subj, obj, pred);
        //
        //     var actual = new Column("");
        //     actual.FromGraph(graph);
        //     
        //     Assert.Equal(expected.BaseUri, actual.BaseUri);
        //     Assert.Equal(expected.Id, actual.Id);
        //     Assert.Equal(expected.Name, actual.Name);
        //     Assert.Equal(expected.DataType, actual.DataType);
        // }
    }
}