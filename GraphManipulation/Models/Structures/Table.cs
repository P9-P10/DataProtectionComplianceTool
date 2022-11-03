using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Table : Structure
{
    public List<Column> ForeignKeys = new();
    public List<Column> PrimaryKeys = new();
    
    // TODO: Check at alle foreignkeys refererer til kolonner i den samme tabel

    public Table(string name) : base(name)
    {
    }

    public void AddPrimaryKey(Column column)
    {
        if (PrimaryKeys.Contains(column))
        {
            return;
        }

        if (!SubStructures.Contains(column))
        {
            throw new StructureException("Column must be in the list of SubStructures to be a valid primary key");
        }

        PrimaryKeys.Add(column);
    }

    public void AddForeignKey(Column fromColumn, Column toColumn)
    {
        if (!ForeignKeys.Contains(fromColumn))
        {
            ForeignKeys.Add(fromColumn);
        }

        if (!SubStructures.Contains(fromColumn))
        {
            throw new StructureException("Column must be in the list of SubStructures to be a valid foreign key");
        }

        fromColumn.SetReferences(toColumn);
    }

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();

        AddPrimaryKeysToGraph(graph);
        AddForeignKeysToGraph(graph);

        return graph;
    }

    private void AddPrimaryKeysToGraph(IGraph graph)
    {
        if (PrimaryKeys.Count == 0)
        {
            throw new GraphBasedException("No primary keys when creating graph");
        }

        var subj = graph.CreateUriNode(Uri);
        var pred = graph.CreateUriNode("ddl:primaryKey");

        foreach (var primaryKey in PrimaryKeys)
        {
            var obj = graph.CreateUriNode(primaryKey.Uri);
            graph.Assert(subj, pred, obj);
        }
    }

    private void AddForeignKeysToGraph(IGraph graph)
    {
        if (ForeignKeys.Count == 0)
        {
            return;
        }

        var table = graph.CreateUriNode(Uri);
        var foreignKeyRelation = graph.CreateUriNode("ddl:foreignKey");

        foreach (var foreignKey in ForeignKeys)
        {
            var from = graph.CreateUriNode(foreignKey.Uri);

            graph.Assert(table, foreignKeyRelation, from);

            var referencesRelation = graph.CreateUriNode("ddl:references");
            var to = graph.CreateUriNode(foreignKey.References.Uri);

            graph.Assert(from, referencesRelation, to);
        }
    }
}