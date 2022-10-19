using System;
using System.IO;
using System.Linq;
using Xunit;
using VDS.RDF;
using GraphManipulation;
using VDS.RDF.Writing;
using VDS.RDF.Parsing;
using Xunit.Abstractions;

namespace Test;

public class GraphManipulationTest
{
    private const string Namespace = "http://www.example.org/test#";
    
    private static Triple OntologyDefinitionTriple()
    {
        IGraph graph = new Graph();

        IUriNode subj = graph.CreateUriNode(UriFactory.Create(Namespace));
        IUriNode pred = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        IUriNode obj = graph.CreateUriNode(UriFactory.Create( NamespaceMapper.OWL + "Ontology"));

        return new Triple(subj, pred, obj);
    }
    

    [Fact]
    public void OntologyToGraphWithoutBaseThrowsException()
    {
        string badOntology = "";
        
        Ontology ontology = new Ontology(new StringReader(badOntology), new TurtleParser());
        
        var exception = Assert.Throws<OntologyException>(() => ontology.ToGraph());
        Assert.Equal("No base defined", exception.Message);
    }

    [Fact]
    public void OntologyToGraphWithOntologyDefinitionIsSuccess()
    {
        Ontology ontology = new Ontology("simpleTestOntology.ttl", new TurtleParser());
        
        IGraph graph = ontology.ToGraph();

        Triple expected = OntologyDefinitionTriple();
        
        Assert.True(graph.ContainsTriple(expected));
    }
    
    [Fact]
    public void OntologyToGraphWithoutOntologyDefinitionThrowsException()
    {
        string badOntology = "@base <http://www.example.org/test#> .";
        
        Ontology ontology = new Ontology(new StringReader(badOntology), new TurtleParser());

        var exception = Assert.Throws<OntologyException>(() => ontology.ToGraph());
        Assert.Equal("Missing ontology definition", exception.Message);
    }

    [Fact]
    public void AddTripleToGraph()
    {
        IGraph testGraph = new Graph();

        IUriNode subj = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        IUriNode pred = testGraph.CreateUriNode(UriFactory.Create("http://www.noget.org/"));
        ILiteralNode obj = testGraph.CreateLiteralNode("Hello");

        testGraph.Assert(new Triple(subj, pred, obj));

        Assert.Equal(1, testGraph.Triples.Count);
        Assert.Equal(new Triple(subj, pred, obj), testGraph.Triples.First());
    }
    
    [Fact]
    public void EqualUriNodesComparedIsTrue()
    {
        IGraph testGraph = new Graph();

        IUriNode a = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        IUriNode b = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        
        Assert.Equal(a, b);
    }
    
    [Fact]
    public void NotEqualUriNodesComparedIsFalse()
    {
        IGraph testGraph = new Graph();

        IUriNode a = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        IUriNode b = testGraph.CreateUriNode(UriFactory.Create("http://www.noget.org/"));
        
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void GraphWriteToAndReadFromFileIsPossible()
    {
        IGraph graphToBeWritten = new Graph();

        IUriNode subj = graphToBeWritten.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        IUriNode pred = graphToBeWritten.CreateUriNode(UriFactory.Create("http://www.noget.org/"));
        ILiteralNode obj = graphToBeWritten.CreateLiteralNode("Hello");

        graphToBeWritten.Assert(new Triple(subj, pred, obj));

        IRdfWriter writer = new CompressingTurtleWriter();
        writer.Save(graphToBeWritten, "test.ttl");

        IGraph parsedGraph = new Graph();
        
        IRdfReader parser = new TurtleParser();
        parser.Load(parsedGraph, "test.ttl");
        
        Assert.Equal(new Triple(subj, pred, obj), parsedGraph.Triples.First());
    }
}