using System.IO;
using Xunit;
using VDS.RDF;
using GraphManipulation;
using VDS.RDF.Parsing;

namespace Test;

public class OntologyTest
{
    private const string Namespace = "http://www.example.org/test#";

    private static Triple OntologyDefinitionTriple()
    {
        IGraph graph = new Graph();

        IUriNode subj = graph.CreateUriNode(UriFactory.Create(Namespace));
        IUriNode pred = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        IUriNode obj = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.OWL + "Ontology"));

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
}