using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Column : Structure
{
    public Column(string name) : base(name)
    {
        DataType = "";
    }

    public Column(string name, string dataType) : base(name)
    {
        DataType = dataType;
    }

    public string DataType { get; private set; }
    public Column? References { get; private set; }

    public void SetReferences(Column column)
    {
        References = column;
    }

    public void SetDataType(string dataType)
    {
        DataType = dataType;
    }

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();

        AddDataTypeToGraph(graph);

        return graph;
    }

    private void AddDataTypeToGraph(IGraph graph)
    {
        var subj = graph.CreateUriNode(Uri);
        var pred = graph.CreateUriNode("ddl:hasDataType");
        var obj = graph.CreateLiteralNode(DataType);

        graph.Assert(subj, pred, obj);
    }

    protected override string GetGraphTypeString()
    {
        return "Column";
    }
}