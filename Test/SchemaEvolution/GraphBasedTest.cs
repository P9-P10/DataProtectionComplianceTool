using GraphManipulation.SchemaEvolution.Extensions;
using GraphManipulation.SchemaEvolution.Models.Stores;
using GraphManipulation.SchemaEvolution.Models.Structures;
using GraphManipulation.SchemaEvolution.Ontologies;
using VDS.RDF;
using Xunit;

namespace Test.SchemaEvolution;

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

            // Added to avoid StructureException caused by Structure having no Database
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

            // Added to avoid StructureException caused by Structure having no Database
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            Assert.True(graph.NamespaceMap.HasNamespace("rdf"));
            Assert.True(graph.NamespaceMap.HasNamespace(DatabaseDescriptionLanguage.OntologyPrefix));
        }

        [Fact]
        public void EntityWithoutBaseThrowsException()
        {
            var column = new Column("Column");

            Assert.Throws<DatabaseToGraphException>(() => column.ToGraph());
        }

        [Fact]
        public void NamedEntityReturnsGraphWithName()
        {
            const string columnName = "MyColumn";
            var column = new Column(columnName);
            column.UpdateBaseUri(baseUri);

            // Added to avoid StructureException caused by Structure having no Database
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(column.Uri);
            var pred = graph.CreateUriNode(DatabaseDescriptionLanguage.HasName);
            var obj = graph.CreateLiteralNode(columnName);

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }

        [Fact]
        public void EntityReturnGraphWithType()
        {
            const string columnName = "MyColumn";
            var column = new Column(columnName);
            column.UpdateBaseUri(baseUri);

            // Added to avoid StructureException caused by Structure having no Database
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(column.Uri);
            var pred = graph.CreateUriNode("rdf:type");
            var obj = graph.CreateUriNode(DatabaseDescriptionLanguage.Column);

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }

        [Fact]
        public void StructuredEntityReturnsGraphWithHasStructure()
        {
            var parent = new Column("Parent");
            var child = new Column("Child");

            parent.UpdateBaseUri(baseUri);
            parent.AddStructure(child);

            // Added to avoid StructureException caused by Structure having no Database
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(parent);

            var graph = parent.ToGraph();

            var triple = new Triple(
                graph.CreateUriNode(parent.Uri),
                graph.CreateUriNode(DatabaseDescriptionLanguage.HasStructure),
                graph.CreateUriNode(child.Uri)
            );

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void DatabaseSubStructuresAddedToGraph()
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
                graph.CreateUriNode(DatabaseDescriptionLanguage.HasStructure),
                graph.CreateUriNode(table1.Uri)
            );

            var triple2 = new Triple(
                graph.CreateUriNode(sqlite.Uri),
                graph.CreateUriNode(DatabaseDescriptionLanguage.HasStructure),
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

            // Added to avoid StructureException caused by Structure having no Database
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(schema);

            var graph = schema.ToGraph();

            var triple1 = new Triple(
                graph.CreateUriNode(schema.Uri),
                graph.CreateUriNode(DatabaseDescriptionLanguage.HasStructure),
                graph.CreateUriNode(table.Uri)
            );

            var triple2 = new Triple(
                graph.CreateUriNode(table.Uri),
                graph.CreateUriNode(DatabaseDescriptionLanguage.HasStructure),
                graph.CreateUriNode(column1.Uri)
            );

            var triple3 = new Triple(
                graph.CreateUriNode(table.Uri),
                graph.CreateUriNode(DatabaseDescriptionLanguage.HasStructure),
                graph.CreateUriNode(column2.Uri)
            );

            Assert.Contains(triple1, graph.Triples);
            Assert.Contains(triple2, graph.Triples);
            Assert.Contains(triple3, graph.Triples);
        }

        [Fact]
        public void StructureHasDatabaseAddedToGraph()
        {
            var sqlite = new Sqlite("SQLite");
            var column = new Column("Column");

            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = sqlite.ToGraph();

            var triple = new Triple(
                graph.CreateUriNode(column.Uri),
                graph.CreateUriNode(DatabaseDescriptionLanguage.HasDatabase),
                graph.CreateUriNode(sqlite.Uri)
            );

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void StructureHasNoDatabaseWhenBuildingGraphThrowsException()
        {
            var column = new Column("Column");

            column.UpdateBaseUri(baseUri);

            Assert.Throws<DatabaseToGraphException>(() => column.ToGraph());
        }
    }
}