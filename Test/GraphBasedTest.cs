using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using VDS.RDF;
using Xunit;

namespace Test;

public class GraphBasedTest
{
    private const string baseURI = "http://www.test.com/";
    
    public class ToGraph
    {
        // TODO: UpdateBase -> UpdateBaseUri
        // TODO: Base -> UriBase

        [Fact]
        public void ResultingGraphHasBaseUri()
        {
            var column = new Column("Column");
            column.UpdateBase(baseURI);

            IGraph graph = column.ToGraph();
            
            Assert.Equal(UriFactory.Create(baseURI), graph.BaseUri);
        }
        
        [Fact]
        public void ResultingGraphHasOntologyNamespace()
        {
            var column = new Column("Column");
            column.UpdateBase(baseURI);

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
            column.UpdateBase(baseURI);
            
            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(UriFactory.Create(column.Base + column.Id));
            var pred = graph.CreateUriNode("ddl:hasName");
            var obj = graph.CreateLiteralNode(columnName);

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }

        [Fact]
        public void EntityReturnGraphWithType()
        {
            const string columnName = "MyColumn";
            var column = new Column(columnName);
            column.UpdateBase(baseURI);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(UriFactory.Create(column.Base + column.Id));
            var pred = graph.CreateUriNode("rdf:type");
            var obj = graph.CreateUriNode("ddl:Column");

            Assert.Contains(new Triple(subj, pred, obj), graph.Triples);
        }
        
        [Fact]
        public void StoreStructuresAddedToGraph()
        {
            var sqlite = new Sqlite("SQLite");
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            
            sqlite.UpdateBase(baseURI);
            sqlite.AddStructure(table1);
            sqlite.AddStructure(table2);

            var graph = sqlite.ToGraph();
            
            var triple1 = new Triple(
                graph.CreateUriNode(UriFactory.Create(sqlite.Base + sqlite.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(table1.Base + table1.Id))
            );
            
            var triple2 = new Triple(
                graph.CreateUriNode(UriFactory.Create(sqlite.Base + sqlite.Id)), 
                graph.CreateUriNode("ddl:hasStructure"), 
                graph.CreateUriNode(UriFactory.Create(table2.Base + table2.Id))
            );

            Assert.Contains(triple1, graph.Triples);
            Assert.Contains(triple2, graph.Triples);
        }
    }

    public class FromGraph
    {
        
    }
}