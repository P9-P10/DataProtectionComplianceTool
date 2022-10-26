using System.Text;
using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using J2N.Text;
using VDS.RDF;
using Xunit;

namespace Test;

public class GraphBasedTest
{
    private const string baseURI = "http://www.test.com/";
    
    public class ToGraph
    {
        [Fact]
        public void ResultingGraphHasBaseUri()
        {
            var column = new Column("Column");
            column.UpdateBaseUri(baseURI);

            IGraph graph = column.ToGraph();
            
            Assert.Equal(UriFactory.Create(baseURI), graph.BaseUri);
        }
        
        [Fact]
        public void ResultingGraphHasOntologyNamespace()
        {
            var column = new Column("Column");
            column.UpdateBaseUri(baseURI);

            IGraph graph = column.ToGraph();
            
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
            column.UpdateBaseUri(baseURI);
            
            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(UriFactory.Create(column.BaseUri + column.Id));
            var pred = graph.CreateUriNode("ddl:hasName");
            var obj = graph.CreateLiteralNode(columnName);

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }

        [Fact]
        public void EntityReturnGraphWithType()
        {
            const string columnName = "MyColumn";
            var column = new Column(columnName);
            column.UpdateBaseUri(baseURI);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(UriFactory.Create(column.BaseUri + column.Id));
            var pred = graph.CreateUriNode("rdf:type");
            var obj = graph.CreateUriNode("ddl:Column");

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }

        [Fact]
        public void StructuredEntityReturnsGraphWithHasStructure()
        {
            var parent = new Column("Parent");
            var child = new Column("Child");
            
            parent.UpdateBaseUri(baseURI);
            parent.AddStructure(child);

            var graph = parent.ToGraph();
            
            var triple = new Triple(
                graph.CreateUriNode(UriFactory.Create(parent.BaseUri + parent.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(child.BaseUri + child.Id))
            );

            Assert.Contains(triple, graph.Triples);
        }
        
        [Fact]
        public void StoreSubStructuresAddedToGraph()
        {
            var sqlite = new Sqlite("SQLite");
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            
            sqlite.UpdateBaseUri(baseURI);
            sqlite.AddStructure(table1);
            sqlite.AddStructure(table2);

            var graph = sqlite.ToGraph();
            
            var triple1 = new Triple(
                graph.CreateUriNode(UriFactory.Create(sqlite.BaseUri + sqlite.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(table1.BaseUri + table1.Id))
            );
            
            var triple2 = new Triple(
                graph.CreateUriNode(UriFactory.Create(sqlite.BaseUri + sqlite.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(table2.BaseUri + table2.Id))
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
            
            schema.UpdateBaseUri(baseURI);
            schema.AddStructure(table);
            table.AddStructure(column1);
            table.AddStructure(column2);

            var graph = schema.ToGraph();
            
            var triple1 = new Triple(
                graph.CreateUriNode(UriFactory.Create(schema.BaseUri + schema.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(table.BaseUri + table.Id))
            );
            
            var triple2 = new Triple(
                graph.CreateUriNode(UriFactory.Create(table.BaseUri + table.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(column1.BaseUri + column1.Id))
            );
            
            var triple3 = new Triple(
                graph.CreateUriNode(UriFactory.Create(table.BaseUri + table.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(column2.BaseUri + column2.Id))
            );
            
            Assert.Contains(triple1, graph.Triples);
            Assert.Contains(triple2, graph.Triples);
            Assert.Contains(triple3, graph.Triples);
        }
    }

    public class FromGraph
    {
        
    }
}