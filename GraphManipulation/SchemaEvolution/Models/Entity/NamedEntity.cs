namespace GraphManipulation.SchemaEvolution.Models.Entity;

public abstract class NamedEntity : SchemaEvolution.Models.Entity.Entity
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