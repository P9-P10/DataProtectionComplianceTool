namespace GraphManipulation.Models.Stores;

public abstract class Relational : Database
{
    protected Relational(string name) : base(name)
    {
    }
}