using System.Data.Common;
using GraphManipulation.SchemaEvolution.Models.Entity;
using GraphManipulation.SchemaEvolution.Models.Structures;

namespace GraphManipulation.SchemaEvolution.Models.Stores;

public abstract class Database : StructuredEntity
{
    protected DbConnection? Connection;

    protected Database(string name) : base(name)
    {
    }

    protected Database(string name, string baseUri) : this(name)
    {
        BaseUri = baseUri;
        ComputeId();
    }

    protected Database(string name, string baseUri, DbConnection connection) : this(name, baseUri)
    {
        Connection = connection;
    }

    public void SetConnection(DbConnection connection)
    {
        Connection = connection;
    }

    public virtual void BuildFromDataSource()
    {
        BuildDatabase();
    }

    private void BuildDatabase()
    {
    }

    public override void AddStructure(Structure structure)
    {
        if (SubStructures.Contains(structure))
        {
            return;
        }

        SubStructures.Add(structure);

        if (!structure.HasSameDatabase(this))
        {
            structure.UpdateDatabase(this);
        }

        if (HasBase() && !structure.HasSameBase(BaseUri!))
        {
            structure.UpdateBaseUri(BaseUri!);
        }

        structure.UpdateIdToBottom();
    }

    public override List<string> ConstructIdString()
    {
        return new List<string> { Name };
    }

    public override void UpdateBaseUri(string baseUri)
    {
        BaseUri = baseUri;
        ComputeId();

        foreach (var structure in SubStructures.Where(structure => !structure.HasSameBase(baseUri)))
            structure.UpdateBaseUri(baseUri);
    }
}

public class DatabaseException : Exception
{
    public DatabaseException(string message) : base(message)
    {
    }
}