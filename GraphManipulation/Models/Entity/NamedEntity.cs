namespace GraphManipulation.Models.Entity;

public abstract class NamedEntity : Entity
{
    protected NamedEntity(string name) : base(name)
    {
        Name = name;
    }

    public string Name { get; protected set; }

    // TODO: Dette skal også opdatere børns ID
    public void UpdateName(string name)
    {
        Name = name;
    }
}