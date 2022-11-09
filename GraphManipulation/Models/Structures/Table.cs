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

        var onActionsAreSame =
            relevantKeys.All(fk => fk.OnDelete == foreignKey.OnDelete && fk.OnUpdate == foreignKey.OnUpdate);

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
}