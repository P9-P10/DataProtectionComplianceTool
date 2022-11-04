using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public class Table : Structure
{
    public List<ForeignKey> ForeignKeys = new();
    public List<Column> PrimaryKeys = new();

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

        CheckForeignKeysWithSameToParentHaveSameOnActions(foreignKey);

        if (!ForeignKeys.Contains(foreignKey))
        {
            ForeignKeys.Add(foreignKey);
        }
    }

    private void CheckForeignKeysWithSameToParentHaveSameOnActions(ForeignKey foreignKey)
    {
        var relevantKeys = ForeignKeys.Where(fk => fk.To.ParentStructure.Equals(foreignKey.To.ParentStructure));

        var onActionsAreSame = relevantKeys.All(fk => fk.OnDelete == foreignKey.OnDelete && fk.OnUpdate == foreignKey.OnUpdate);

        if (!onActionsAreSame)
        {
            throw new StructureException("All foreign keys must have the same 'on actions'");
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
        var foreignKeyPredicate = graph.CreateUriNode("ddl:foreignKey");
        var referencesPredicate = graph.CreateUriNode("ddl:references");
        var foreignKeyOnDeletePredicate = graph.CreateUriNode("ddl:foreignKeyOnDelete");
        var foreignKeyOnUpdatePredicate = graph.CreateUriNode("ddl:foreignKeyOnUpdate");

        foreach (var foreignKey in ForeignKeys)
        {
            var from = graph.CreateUriNode(foreignKey.From.Uri);
            var to = graph.CreateUriNode(foreignKey.To.Uri);
            
            graph.Assert(table, foreignKeyPredicate, from);
            graph.Assert(from, referencesPredicate, to);

            var onDelete = graph.CreateLiteralNode(foreignKey.OnDeleteString);
            var onUpdate = graph.CreateLiteralNode(foreignKey.OnUpdateString);

            graph.Assert(from, foreignKeyOnDeletePredicate, onDelete);
            graph.Assert(from, foreignKeyOnUpdatePredicate, onUpdate);
        }
    }
}