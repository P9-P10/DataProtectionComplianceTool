using System.IO;
using GraphManipulation.Models.Graphs;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Shacl;
using Xunit;

namespace Test;

public class DataGraphTest
{
    private static string getSimpleOntology()
    {
        return @"
            @prefix rdf:   <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .
            @prefix rdfs:  <http://www.w3.org/2000/01/rdf-schema#> .
            @prefix xsd:   <http://www.w3.org/2001/XMLSchema#> .
            @prefix owl:   <http://www.w3.org/2002/07/owl#> .
            @prefix sh:    <http://www.w3.org/ns/shacl#> .
            @prefix sto:   <http://www.example.org/test#> .
            @base <http://www.example.org/test#> .

            sto:
                a owl:Ontology .

            sto:TestProperty
                a rdf:Property .

            sto:TestClass
                a           rdfs:Class, sh:NodeShape ;
                sh:property [ sh:path     sto:TestProperty ;
                              sh:class    sto:TestClass ;
                              sh:minCount 1 ; ] .
        ";
    }

    [Fact]
    public void DataGraphDoesNotConformToPathThrowsException()
    {
        const string data = @"
            @prefix sto:   <http://www.example.org/test#> .
            @base          <http://www.example.org/test#> .
            <T> a sto:TestClass .
        ";

        var dataGraph = new DataGraph(new StringReader(""), data, new TurtleParser());

        IGraph shapesGraph = new Graph();
        shapesGraph.LoadFromString(getSimpleOntology(), new TurtleParser());

        dataGraph.AddShapesGraph(new ShapesGraph(shapesGraph));

        var exception = Assert.Throws<DataGraphException>(() => dataGraph.ToGraph());
        Assert.Contains("Conforms", exception.Message);
        Assert.Contains("False", exception.Message);
        Assert.Contains("(1)", exception.Message);
        Assert.Contains("Focus node: http://www.example.org/T", exception.Message);
        Assert.Contains("Result path: http://www.example.org/test#TestProperty", exception.Message);
        Assert.Contains("Message", exception.Message);
    }

    [Fact]
    public void DataGraphDoesNotConformToValueThrowsException()
    {
        const string data = @"
            @prefix sto:   <http://www.example.org/test#> .
            @base          <http://www.example.org/test#> .
            <T> a sto:TestClass .
            <T> sto:TestProperty 1 .
        ";

        var dataGraph = new DataGraph(new StringReader(""), data, new TurtleParser());

        IGraph shapesGraph = new Graph();
        shapesGraph.LoadFromString(getSimpleOntology(), new TurtleParser());

        dataGraph.AddShapesGraph(new ShapesGraph(shapesGraph));

        var exception = Assert.Throws<DataGraphException>(() => dataGraph.ToGraph());
        Assert.Contains("Conforms", exception.Message);
        Assert.Contains("False", exception.Message);
        Assert.Contains("(1)", exception.Message);
        Assert.Contains("Focus node: http://www.example.org/T", exception.Message);
        Assert.Contains("Result path: http://www.example.org/test#TestProperty", exception.Message);
        Assert.Contains("Result value: 1", exception.Message);
        Assert.Contains("Message", exception.Message);
    }

    [Fact]
    public void DataGraphDoesNotConformTwoViolationsThrowsExceptionWithTwoResults()
    {
        const string data = @"
            @prefix sto:   <http://www.example.org/test#> .
            @base          <http://www.example.org/test#> .
            <T> a sto:TestClass .
            <T> sto:TestProperty 1 .

            <B> a sto:TestClass .
        ";

        var dataGraph = new DataGraph(new StringReader(""), data, new TurtleParser());

        IGraph shapesGraph = new Graph();
        shapesGraph.LoadFromString(getSimpleOntology(), new TurtleParser());

        dataGraph.AddShapesGraph(new ShapesGraph(shapesGraph));

        var exception = Assert.Throws<DataGraphException>(() => dataGraph.ToGraph());
        Assert.Contains("Conforms", exception.Message);
        Assert.Contains("False", exception.Message);
        Assert.Contains("(2)", exception.Message);
        Assert.Contains("Focus node: http://www.example.org/T", exception.Message);
        Assert.Contains("Focus node: http://www.example.org/B", exception.Message);
    }

    [Fact]
    public void DataGraphConformsGraphIsCreated()
    {
        const string data = @"
            @prefix sto:   <http://www.example.org/test#> .
            @base          <http://www.example.org/test#> .
            <T> a sto:TestClass .
            <T> sto:TestProperty <T> .
        ";

        var dataGraph = new DataGraph(new StringReader(""), data, new TurtleParser());

        IGraph shapesGraph = new Graph();
        shapesGraph.LoadFromString(getSimpleOntology(), new TurtleParser());

        dataGraph.AddShapesGraph(new ShapesGraph(shapesGraph));

        var graph = dataGraph.ToGraph();

        var subj = graph.CreateUriNode(UriFactory.Create("http://www.example.org/T"));
        var pred = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        var obj = graph.CreateUriNode(UriFactory.Create("http://www.example.org/test#TestClass"));

        Assert.True(graph.ContainsTriple(new Triple(subj, pred, obj)));
    }

    [Fact]
    public void DataGraphConformsWithoutShapesButDoesNotConformWithShapes()
    {
        const string data = @"
            @prefix sto:   <http://www.example.org/test#> .
            @base          <http://www.example.org/test#> .
            <T> a sto:TestClass .
        ";

        var dataGraph = new DataGraph(new StringReader(""), data, new TurtleParser());

        var graph = dataGraph.ToGraph();

        var subj = graph.CreateUriNode(UriFactory.Create("http://www.example.org/T"));
        var pred = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        var obj = graph.CreateUriNode(UriFactory.Create("http://www.example.org/test#TestClass"));

        Assert.True(graph.ContainsTriple(new Triple(subj, pred, obj)));

        IGraph shapesGraph = new Graph();
        shapesGraph.LoadFromString(getSimpleOntology(), new TurtleParser());

        dataGraph.AddShapesGraph(new ShapesGraph(shapesGraph));

        Assert.Throws<DataGraphException>(() => dataGraph.ToGraph());
    }
}