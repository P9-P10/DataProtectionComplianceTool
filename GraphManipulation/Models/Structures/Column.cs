using VDS.RDF;
using VDS.RDF.Parsing;

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

    public Column(string name, string dataType, bool isNotNull) : this(name, dataType)
    {
        IsNotNull = isNotNull;
    }

    public string DataType { get; private set; }
    
    // TODO: Test dette i forhold til grafen
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

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();

        AddDataTypeToGraph(graph);
        AddIsNotNullToGraph(graph);

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

    protected override string GetGraphTypeString()
    {
        return "Column";
    }
    
    // Lav det her til en extension method
    public static Column GetColumnFromTable(string columnName, Table table)
    {
        return (table.SubStructures.First(s => s.Name == columnName) as Column)!;
    }
}