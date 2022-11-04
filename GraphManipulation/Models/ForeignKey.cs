using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models;

public enum ForeignKeyOnEnum
{
    NoAction, Cascade
}

public class ForeignKey
{
    private string ForeignKeyEnumToString(ForeignKeyOnEnum e)
    {
        switch (e)
        {
            case ForeignKeyOnEnum.Cascade:
                return "CASCADE";
            case ForeignKeyOnEnum.NoAction:
                return "NO ACTION";
            default:
                throw new ForeignKeyException("Enum not supported: " + OnDelete);
        }
    }

    public string OnDeleteString => ForeignKeyEnumToString(OnDelete);
    public string OnUpdateString => ForeignKeyEnumToString(OnUpdate);

    public ForeignKeyOnEnum OnDelete { get; }
    public ForeignKeyOnEnum OnUpdate { get; }
    public Column From { get; }
    public Column To { get; }

    public ForeignKey(Column from, Column to, 
        ForeignKeyOnEnum onDelete = ForeignKeyOnEnum.NoAction,
        ForeignKeyOnEnum onUpdate = ForeignKeyOnEnum.NoAction)
    {
        if (from.ParentStructure is null || to.ParentStructure is null)
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

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        
        return obj.GetType() == typeof(ForeignKey) && From.Equals((obj as ForeignKey).From);
    }

    public override int GetHashCode()
    {
        return From.GetHashCode();
    }
}

public class ForeignKeyException : Exception
{
    public ForeignKeyException(string message) : base(message)
    {
    }
}