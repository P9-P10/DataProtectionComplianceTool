using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Table : Structure
{
    public List<Column> PrimaryKeys = new List<Column>();
    public List<Column> ForeignKeys = new List<Column>();
    
    public Table(string name) : base(name)
    {
    }

    public void AddPrimaryKey(Column column)
    {
        if (PrimaryKeys.Contains(column)) return;
        
        PrimaryKeys.Add(column);
    }

    public void AddForeignKey(Column fromColumn, Column toColumn)
    {
        if (!ForeignKeys.Contains(fromColumn))
        {
            ForeignKeys.Add(fromColumn);
        }

        fromColumn.SetReferences(toColumn);
    }

    public override IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();
        return graph;
    }
    
    protected override string GetGraphTypeString()
    {
        return "Table";
    }
}