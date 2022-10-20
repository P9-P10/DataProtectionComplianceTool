namespace GraphManipulation.Models.Stores;

public abstract class Database : DataStore
{
    protected Database(string name) : base(name)
    {
    }
}