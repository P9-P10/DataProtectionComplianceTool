using System.IO;
using GraphManipulation.Models;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Shacl;
using Xunit;

namespace Test;

public class DataGraphTest
{
    [Fact]
    public void DataGraphMustConformToShapes()
    {
        IGraph graph = new Graph();

        string data = @"
            @prefix sto:   <http://www.example.org/test#> .
            @base          <http://www.example.org/test#> .
            sto:T rdf:type rdfs:Class .
        ";

        DataGraph dataGraph = new DataGraph(new StringReader(data), new TurtleParser());
        
        // TODO: Tilføj shapes til simpleTestOntology som vi kan validere med
        
        IGraph shapesGraph = new Graph();
        shapesGraph.LoadFromFile("simpleTestOntology.ttl", new TurtleParser());

        dataGraph.AddShapesGraph(new ShapesGraph(shapesGraph));

        graph = dataGraph.ToGraph();
    }
}