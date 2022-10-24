using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Column : Structure
{
    public string DataType { get; private set; }
    public Column? References { get; private set; }
    
    public Column(string name) : base(name)
    {
        DataType = "";
    }

    public Column(string name, string dataType) : base(name)
    {
        DataType = dataType;
    }

    public void SetReferences(Column column)
    {
        References = column;
    }

    public void SetDataType(string dataType)
    {
        DataType = dataType;
    }

    public new IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();
        return graph;
    }

    protected override string GetGraphTypeString()
    {
        return "Column";
    }
}