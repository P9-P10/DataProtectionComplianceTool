namespace GraphManipulation.Models.Stores;

public abstract class Relational : Database
{
    protected Relational(string name) : base(name)
    {
    }
    
    public override void Build()
    {
        base.Build();
        BuildRelational();
    }

    private void BuildRelational()
    {
        
    }
}