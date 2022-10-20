using System.IO;
using GraphManipulation.Models.Graphs;
using VDS.RDF;
using VDS.RDF.Parsing;
using Xunit;

namespace Test;

public class OntologyTest
{
    private const string Namespace = "http://www.example.org/test#";

    private static Triple OntologyDefinitionTriple()
    {
        IGraph graph = new Graph();

        var subj = graph.CreateUriNode(UriFactory.Create(Namespace));
        var pred = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        var obj = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.OWL + "Ontology"));

        return new Triple(subj, pred, obj);
    }

    [Fact]
    public void OntologyToGraphWithoutBaseThrowsException()
    {
        const string badOntology = "";

        var ontology = new Ontology(new StringReader(""), badOntology, new TurtleParser());

        var exception = Assert.Throws<KnowledgeGraphException>(() => ontology.ToGraph());
        Assert.Equal("No base defined", exception.Message);
    }


    [Fact]
    public void OntologyToGraphWithOntologyDefinitionIsSuccess()
    {
        var ontology = new Ontology("simpleTestOntology.ttl", new TurtleParser());

        var graph = ontology.ToGraph();

        var expected = OntologyDefinitionTriple();

        Assert.True(graph.ContainsTriple(expected));
    }

    [Fact]
    public void OntologyToGraphWithoutOntologyDefinitionThrowsException()
    {
        const string badOntology = "@base <http://www.example.org/test#> .";

        var ontology = new Ontology(new StringReader(""), badOntology, new TurtleParser());

        var exception = Assert.Throws<OntologyException>(() => ontology.ToGraph());
        Assert.Equal("Missing ontology definition", exception.Message);
    }

    [Fact]
    public void OntologyToGraphWithBadSyntaxThrowsException()
    {
        const string badOntology = "@base <http://www.example.org/test#> . something is wrong here . ";

        var ontology = new Ontology(new StringReader(""), badOntology, new TurtleParser());

        Assert.Throws<RdfParseException>(() => ontology.ToGraph());
    }
}