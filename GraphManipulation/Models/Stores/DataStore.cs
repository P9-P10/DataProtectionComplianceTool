using System.Data;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Stores;

public enum SupportedDataStores
{
    Sqlite,
    PostgreSql,
    MongoDb,
    Csv
}

public abstract class DataStore : StructuredEntity
{
    protected DataStore(string name) : base(name)
    {
    }

    protected DataStore(string name, string baseUri) : this(name)
    {
        BaseUri = baseUri;
        ComputeId();
    }

    // protected SupportedDataStores DataStoreType;

    public override void AddStructure(Structure structure)
    {
        if (SubStructures.Contains(structure))
        {
            return;
        }

        SubStructures.Add(structure);

        if (!structure.HasSameStore(this))
        {
            structure.UpdateStore(this);
        }

        if (HasBase() && !structure.HasSameBase(BaseUri!))
        {
            structure.UpdateBaseUri(BaseUri!);
        }

        structure.UpdateIdToBottom();
    }

    public virtual void BuildFromDataSource()
    {
        BuildDataStore();
    }

    private void BuildDataStore()
    {
    }

    public override List<string> ConstructIdString()
    {
        return new List<string> { Name };
    }

    public override void UpdateBaseUri(string baseUri)
    {
        BaseUri = baseUri;
        ComputeId();

        foreach (var structure in SubStructures)
            if (!structure.HasSameBase(baseUri))
            {
                structure.UpdateBaseUri(baseUri);
            }
    }
}

public class DataStoreException : Exception
{
    public DataStoreException(string message) : base(message)
    {
    }
}