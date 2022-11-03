using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Table : Structure
{
    public List<ForeignKey> ForeignKeys = new();
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
        AddForeignKey(new ForeignKey(fromColumn, toColumn));
    }

    public void AddForeignKey(ForeignKey foreignKey)
    {
        if (!SubStructures.Contains(foreignKey.From))
        {
            throw new StructureException("The 'from' part of a foreign key must be in the list of SubStructures");
        }

        if (!ForeignKeys.Contains(foreignKey))
        {
            ForeignKeys.Add(foreignKey);
        }
    }

    public void DeleteForeignKey(ForeignKey foreignKey)
    {
        ForeignKeys.Remove(foreignKey);
    }

    public void DeleteForeignKey(Column from)
    {
        DeleteForeignKey(from.Name);
    }

    public void DeleteForeignKey(string fromName)
    {
        ForeignKeys.Remove(ForeignKeys.First(fk => fk.From.Name == fromName));
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
            var from = graph.CreateUriNode(foreignKey.From.Uri);

            graph.Assert(table, foreignKeyRelation, from);

            var referencesRelation = graph.CreateUriNode("ddl:references");
            var to = graph.CreateUriNode(foreignKey.To.Uri);

            graph.Assert(from, referencesRelation, to);
        }
    }
}