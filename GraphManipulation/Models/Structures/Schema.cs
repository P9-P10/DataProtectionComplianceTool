using GraphManipulation.Models.Stores;
using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Schema : Structure
{
    public Schema(string name) : base(name)
    {
    }

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();
        return graph;
    }

    protected override string GetGraphTypeString()
    {
        return "Schema";
    }
    
    public static Schema GetSchemaFromDatastore(string schemaName, DataStore store)
    {
        return (store.SubStructures.First(s => s.Name == schemaName) as Schema)!;
    }
}