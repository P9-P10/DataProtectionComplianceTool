using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models;

public enum ForeignKeyOnEnum
{
    NoAction, Cascade
}

public class ForeignKey
{
    public ForeignKeyOnEnum OnDelete { get; }
    public ForeignKeyOnEnum OnUpdate { get; }
    public Column From { get; }
    public Column To { get; }

    public ForeignKey(Column from, Column to, ForeignKeyOnEnum onDelete = ForeignKeyOnEnum.NoAction,
        ForeignKeyOnEnum onUpdate = ForeignKeyOnEnum.NoAction)
    {
        if (from.ParentStructure == null || to.ParentStructure == null)
        {
            throw new ForeignKeyException("Parent cannot be null");
        }

        if (from.ParentStructure.Equals(to.ParentStructure))
        {
            throw new ForeignKeyException("Parents cannot be the same");
        }

        From = from;
        To = to;
        OnDelete = onDelete;
        OnUpdate = onUpdate;
    }
}

public class ForeignKeyException : Exception
{
    public ForeignKeyException(string message) : base(message)
    {
    }
}