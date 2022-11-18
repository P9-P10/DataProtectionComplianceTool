namespace GraphManipulation.Models.Entity;

public abstract class NamedEntity : Entity
{
    protected NamedEntity(string name) : base(name)
    {
        Name = name;
    }

    public string Name { get; protected set; }

    public virtual void UpdateName(string name)
    {
        Name = name;
    }
}