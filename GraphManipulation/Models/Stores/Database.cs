using System.Data;
using System.Data.Common;

namespace GraphManipulation.Models.Stores;

public abstract class Database : DataStore
{
    protected DbConnection? Connection;
    
    protected Database(string name) : base(name)
    {
    }

    protected Database(string name, string baseUri) : base(name, baseUri)
    {
        
    }

    protected Database(string name, string baseUri, DbConnection connection) : this(name, baseUri)
    {
        Connection = connection;
    }
    
    public void SetConnection(DbConnection connection)
    {
        Connection = connection;
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