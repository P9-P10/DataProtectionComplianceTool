using VDS.RDF;
using VDS.RDF.Parsing;

namespace GraphManipulation.Models.Structures;

public class Column : Structure
{
    public Column(string name, string dataType = "", bool isNotNull = false, string options = "") : base(name)
    {
        DataType = dataType;
        IsNotNull = isNotNull;
        Options = options;
    }

    public string Options { get; private set; }
    public string DataType { get; private set; }
    public bool IsNotNull { get; private set; }
    public Column? References { get; private set; }

    public void SetReferences(Column column)
    {
        References = column;
    }

    public void SetDataType(string dataType)
    {
        DataType = dataType;
    }

    public void SetIsNotNull(bool isNotNull)
    {
        IsNotNull = isNotNull;
    }

    public void SetOptions(string options)
    {
        Options = options;
    }

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();

        AddDataTypeToGraph(graph);
        AddIsNotNullToGraph(graph);
        AddOptionsToGraph(graph);

        return graph;
    }

    private void AddDataTypeToGraph(IGraph graph)
    {
        var triple = new Triple(
            graph.CreateUriNode(Uri),
            graph.CreateUriNode("ddl:hasDataType"),
            graph.CreateLiteralNode(DataType)
        );

        graph.Assert(triple);
    }

    private void AddIsNotNullToGraph(IGraph graph)
    {
        var triple = new Triple(
            graph.CreateUriNode(Uri),
            graph.CreateUriNode("ddl:isNotNull"),
            graph.CreateLiteralNode(IsNotNull.ToString().ToLower(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeBoolean)));

        graph.Assert(triple);
    }

    private void AddOptionsToGraph(IGraph graph)
    {
        var triple = new Triple(
            graph.CreateUriNode(Uri),
            graph.CreateUriNode("ddl:columnOptions"),
            graph.CreateLiteralNode(Options));

        graph.Assert(triple);
    }

    // protected override string GetGraphTypeString()
    // {
    //     return "Column";
    // }
}