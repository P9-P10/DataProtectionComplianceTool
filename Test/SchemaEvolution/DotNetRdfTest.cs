using System.Linq;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using Xunit;

namespace Test.SchemaEvolution;

public class DotNetRdfTest
{
    [Fact]
    public void AddTripleToGraph()
    {
        IGraph testGraph = new Graph();

        var subj = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        var pred = testGraph.CreateUriNode(UriFactory.Create("http://www.noget.org/"));
        var obj = testGraph.CreateLiteralNode("Hello");

        testGraph.Assert(new Triple(subj, pred, obj));

        Assert.Equal(1, testGraph.Triples.Count);
        Assert.Equal(new Triple(subj, pred, obj), testGraph.Triples.First());
    }

    [Fact]
    public void EqualUriNodesComparedIsTrue()
    {
        IGraph testGraph = new Graph();

        var a = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        var b = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));

        Assert.Equal(a, b);
    }

    [Fact]
    public void NotEqualUriNodesComparedIsFalse()
    {
        IGraph testGraph = new Graph();

        var a = testGraph.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        var b = testGraph.CreateUriNode(UriFactory.Create("http://www.noget.org/"));

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void GraphWriteToAndReadFromFileIsPossible()
    {
        IGraph graphToBeWritten = new Graph();

        var subj = graphToBeWritten.CreateUriNode(UriFactory.Create("http://www.test.org/"));
        var pred = graphToBeWritten.CreateUriNode(UriFactory.Create("http://www.noget.org/"));
        var obj = graphToBeWritten.CreateLiteralNode("Hello");

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