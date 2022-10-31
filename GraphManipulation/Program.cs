// See https://aka.ms/new-console-template for more information

using System.Data.SQLite;
using GraphManipulation.Models.Graphs;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using VDS.RDF.Shacl;


string baseUri = "http://www.test.com/";

using var conn = new SQLiteConnection("Data Source=/home/ane/Documents/GitHub/Legeplads/Databases/OptimizedAdvancedDatabase.sqlite");

var sqlite = new Sqlite("", baseUri, conn);

sqlite.Build();

IGraph graph = sqlite.ToGraph();

var writer = new CompressingTurtleWriter();

const string path = "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/test.ttl";

// TODO: Hvordan kan jeg fjerne "hasName" fra grafen, og ikke få at den fejler?
writer.Save(graph, path);

var dataGraph = new DataGraph(path, new TurtleParser());
IGraph shapesGraph = new Graph();

const string shapesPath = "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/Ontologies/datastore-description-language.ttl";

shapesGraph.LoadFromFile(shapesPath, new TurtleParser());
dataGraph.AddShapesGraph(new ShapesGraph(shapesGraph));

dataGraph.Validate();


