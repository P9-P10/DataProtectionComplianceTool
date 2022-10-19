using System.Linq;
using Xunit;
using VDS.RDF;
using VDS.RDF.Writing;
using VDS.RDF.Parsing;

namespace Test;

public class DotNetRdfTest
{
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

        // https://dotnetrdf.org/docs/latest/user_guide/writing_rdf.html
        IRdfWriter writer = new CompressingTurtleWriter();
        writer.Save(graphToBeWritten, "test.ttl");

        IGraph parsedGraph = new Graph();

        // https://dotnetrdf.org/docs/latest/user_guide/reading_rdf.html
        IRdfReader parser = new TurtleParser();
        parser.Load(parsedGraph, "test.ttl");

        Assert.Equal(new Triple(subj, pred, obj), parsedGraph.Triples.First());
    }
}