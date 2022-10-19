using System;
using System.Linq;
using Xunit;
using VDS.RDF;
using GraphManipulation;
using VDS.RDF.Writing;
using VDS.RDF.Parsing;

namespace Test;

public class GraphManipulationTest
{

    [Fact]
    public void DDLOntologyReturnsGraph()
    {
        IGraph graph = new Graph();

        Ontology ontology = new Ontology("simpleTestOntology.ttl", new TurtleParser());
        
        ontology.ToGraph(graph);

        string name = "http://www.example.org/test#";
        string owl = "http://www.w3.org/2002/07/owl#";
        string rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        IUriNode subj = graph.CreateUriNode(UriFactory.Create(name + "sto:"));
        IUriNode pred = graph.CreateUriNode(UriFactory.Create(rdf + "type"));
        IUriNode obj = graph.CreateUriNode(UriFactory.Create( owl + "Ontology"));
        
        
        Assert.True(graph.ContainsTriple(new Triple(subj, pred, obj)));
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