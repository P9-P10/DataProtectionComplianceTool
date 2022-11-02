using System.Data;
using System.Data.Common;

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
    // public override string GetConnectionString()
    // {
    //     if (_connection is null) throw new DataStoreException("Connection was null when building DataStore");
    //
    //     return base.GetConnectionString();
    // }

    protected DbConnection? Connection;
    
    protected Database(string name) : base(name)
    {
        // ConnectionString = "";
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
        // ConnectionString = connection.ConnectionString;
    }
    
    public override void Build()
    {
        base.Build();
        BuildDatabase();
    }

    private void BuildDatabase()
    {
        
    }

    public abstract string ToCreateStatement();

    public abstract void FromCreateStatement(string createStatement);
}