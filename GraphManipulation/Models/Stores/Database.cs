namespace GraphManipulation.Models.Stores;

public abstract class Database : DataStore
{
    // protected string ConnectionString;
    //
    // public void SetConnectionString(string connectionString)
    // {
    //     ConnectionString = connectionString;
    // }
    //
    // public virtual string GetConnectionString()
    // {
    //     return ConnectionString;
    // }
    
    protected Database(string name) : base(name)
    {
        // ConnectionString = "";
    }
    
    public override void Build()
    {
        base.Build();
        BuildDatabase();
    }

    private void BuildDatabase()
    {
        
    }
}